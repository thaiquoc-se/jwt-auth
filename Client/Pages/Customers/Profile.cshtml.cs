using DataTransferObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Pages.Customers
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient client = null!;
        private string ApiUrl = "";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProfileModel(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7151/api/Customer";
            _httpContextAccessor = httpContextAccessor;
        }

        public CustomerDTO Customer { get; set; } = default!;
        public int CustomerID { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                CustomerID = int.Parse(claim!.Value);
                if (CustomerID == 0)
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
            var token = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}/{CustomerID}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var _customer = JsonSerializer.Deserialize<CustomerDTO>(strData, options)!;

            Customer = _customer;
            return Page();
        }
    }
}
