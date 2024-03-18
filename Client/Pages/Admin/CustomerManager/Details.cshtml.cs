using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using DataTransferObject.DTO;

namespace Client.Pages.Admin.CustomerManager
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient client = null!;
        private string ApiUrl = "";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DetailsModel(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7151/api/Customer";
            _httpContextAccessor = httpContextAccessor;
        }

        public CustomerDTO Customer { get; set; } = default!;
        public string Admin { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                Admin = HttpContext.Session.GetString("Admin")!;
                if (Admin != "Admin")
                {
                    return NotFound();
                }
                if (Admin == null)
                {
                    return NotFound();
                }

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
            catch
            {
                NotFound();
            }
            return BadRequest();
        }
    }
}
