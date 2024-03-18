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

namespace Client.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient client;
        private string ApiUrl = "";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterModel(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7151/api/Customer";
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CustomerDTO Customer { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var _customer = new CustomerDTO
                {
                    CustomerFullName = Customer.CustomerFullName,
                    CustomerBirthday = Customer.CustomerBirthday,
                    CustomerStatus = 0,
                    EmailAddress = Customer.EmailAddress,
                    Telephone = Customer.Telephone,
                    Password = Customer.Password,
                };

                string strData = JsonSerializer.Serialize(_customer);
                var contentData = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                var token = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsync(ApiUrl, contentData);
                if (response.IsSuccessStatusCode)
                {
                    ViewData["Message"] = "Register Successfully , please login";
                    return RedirectToPage("./Login");
                }
            }
            catch
            {
                ViewData["ErrorMessage"] = "Fail To Call API";

                return Page();
            }

            return RedirectToPage("./Login");
        }
    }
}
