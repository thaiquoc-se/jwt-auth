using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;
using DataTransferObject.DTO;

namespace Client.Pages.Customers
{
    public class EditProfileModel : PageModel
    {
        private readonly HttpClient client = null!;
        private string ApiUrl = "";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditProfileModel(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7151/api/Customer";
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public CustomerDTO Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var token = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}/{id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var _customer = JsonSerializer.Deserialize<CustomerDTO>(strData, options)!;

            Customer = _customer;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string strData = JsonSerializer.Serialize(Customer);
                var contentData = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"{ApiUrl}?id={id}", contentData);
                if (response.IsSuccessStatusCode)
                {
                    ViewData["Success"] = "Update Success";
                    return RedirectToPage("./Profile");
                }
                ViewData["Error"] = "Update Error";
                return RedirectToPage("./Profile");
            }
            catch
            {
                ViewData["Error"] = "Fail To Call API";
                return RedirectToPage("./Profile");
            }
        }
    }
}
