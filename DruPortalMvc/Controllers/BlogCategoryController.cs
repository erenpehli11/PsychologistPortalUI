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
    public class BlogCategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BlogCategoryController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/categories");
                List<BlogCategoryViewModel> categories = new();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogCategoryViewModel>>>(json);
                    categories = apiResponse?.Data ?? new();
                }

                return Json(categories);
            }
            catch (Exception)
            {
                return Json(new List<BlogCategoryViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryAjax([FromBody] BlogCategoryViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    return Json(new { success = false, message = "Kategori adı gereklidir." });

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Blog/add-category", model);
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
        public async Task<IActionResult> UpdateCategoryAjax([FromBody] BlogCategoryViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    return Json(new { success = false, message = "Kategori adı gereklidir." });

                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Blog/update-category", model);
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
        public async Task<IActionResult> DeleteCategoryAjax([FromBody] BlogCategoryViewModel model)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Blog/delete-category/{model.Id}");
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