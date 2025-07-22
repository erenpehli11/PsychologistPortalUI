using DruPortalMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using DruPortalMvc.Models;

namespace DruPortalMvc.Controllers
{
    public class AboutController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public AboutController(IConfiguration configuration , IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("IgnoreSsl");
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/About/get-about");

            AboutViewModel about = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<AboutViewModel>>>(json);
                 about = apiResponse?.Data?.FirstOrDefault() ?? new AboutViewModel();
            }

            return View(about);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AboutViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/About/add-about", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] AboutViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/About/update-about", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/About/delete-about/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }
    }
}
