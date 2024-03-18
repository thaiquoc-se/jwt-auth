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

namespace Client.Pages.Admin.CustomerManager
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client;
        private string ApiUrl = "";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateModel(IHttpContextAccessor httpContextAccessor)
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
        public string Admin { get; private set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
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
            }
            catch
            {
                NotFound();
            }
            try
            {

                string strData = JsonSerializer.Serialize(Customer);
                var contentData = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                var token = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsync(ApiUrl, contentData);
                if (response.IsSuccessStatusCode)
                {
                    ViewData["Message"] = "Add New Customer successfully";
                    return RedirectToPage("./Index");
                }
                ViewData["Message"] = "Add New Customer Fail";
                return Page();
            }
            catch
            {
                ViewData["ErrorMessage"] = "Fail To Call API";

                return Page();
            }
        }
    }
}
