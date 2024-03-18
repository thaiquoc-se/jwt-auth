using DataTransferObject.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient client;
        private string ApiUrl = "";
        public LoginModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7151/api/Auth/login";
        }
        [BindProperty]
        public LoginVM loginVM { get; set; } = default!;


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string strData = JsonSerializer.Serialize(loginVM);
                    var contentData = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ApiUrl, contentData);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        var customer = JsonSerializer.Deserialize<LoginResponseVM>(responseData)!;
                        var userClaims = new List<Claim>();
                        if (customer.role == "Admin")
                        {
                            HttpContext.Session.SetString("Admin", "Admin");
                            userClaims.Add(new Claim("token", customer.token ?? "Undifined"));
                            var adminIdentity = new ClaimsIdentity(userClaims, "Admin Identity");
                            var adminPrincipal = new ClaimsPrincipal(new[] { adminIdentity });
                            await HttpContext.SignInAsync(adminPrincipal);
                            return RedirectToPage("./Admin/CustomerManager/index");
                        }

                        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, customer.data!.CustomerId.ToString() ?? "Undefined"));
                        userClaims.Add(new Claim("token", customer.token ?? "Undifined"));
                        var userIdentity = new ClaimsIdentity(userClaims, "User Identity");
                        var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                        await HttpContext.SignInAsync(userPrincipal);
                        return RedirectToPage("./Customers/Profile");
                    }
                }
                catch
                {
                    ViewData["ErrorMessage"] = "Fail To Call API";

                    return Page();
                }
            }
            ViewData["ErrorMessage"] = "Login Fail";
            return Page();
        }
    }
}
