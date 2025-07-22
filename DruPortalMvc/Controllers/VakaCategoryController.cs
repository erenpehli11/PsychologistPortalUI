using DruPortalMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DruPortalMvc.Controllers
{
    public class VakaCategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public VakaCategoryController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/categories");
                List<VakaCategoryViewModel> categories = new();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaCategoryViewModel>>>(json);
                    categories = apiResponse?.Data ?? new();
                }

                return Json(categories);
            }
            catch (Exception)
            {
                return Json(new List<VakaCategoryViewModel>());
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddCategoryAjax([FromBody] VakaCategoryViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    return Json(new { success = false, message = "Kategori adı gereklidir." });

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Vaka/add-category", model);
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });
                
                return Json(new { success = false, message = "Kategori eklenemedi." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategoryAjax([FromBody] VakaCategoryViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    return Json(new { success = false, message = "Kategori adı gereklidir." });

                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Vaka/update-category", model);
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });
                
                return Json(new { success = false, message = "Kategori güncellenemedi." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryAjax([FromBody] VakaCategoryViewModel model)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Vaka/delete-category/{model.Id}");
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });
                
                return Json(new { success = false, message = "Kategori silinemedi." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }
    }
} 