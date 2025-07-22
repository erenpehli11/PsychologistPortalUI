using DruPortalMvc.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DruPortalMvc.Controllers
{
    public class VakaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public VakaController(IConfiguration configuration)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);
            _apiBaseUrl = configuration["VakaApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/vakalar");
            List<VakaViewModel> vakalar = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaViewModel>>>(json);
                vakalar = apiResponse?.Data ?? new();
            }

            return View(vakalar);
        }


        private async Task<List<SelectListItem>> GetCategoriesForDropdown(Guid? selectedId = null)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/categories");
            List<VakaCategoryViewModel> categories = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaCategoryViewModel>>>(json);
                categories = apiResponse?.Data ?? new();
            }

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedId.HasValue && c.Id == selectedId.Value
            }).ToList();
        }


        public async Task<IActionResult> Create()
        {
            var model = new VakaViewModel
            {
                Categories = await GetCategoriesForDropdown()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VakaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Vaka/add-vaka", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            }
            model.Categories = await GetCategoriesForDropdown();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/vakalar");
            VakaViewModel vaka = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaViewModel>>>(json);
                vaka = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (vaka == null) return NotFound();
            vaka.Categories = await GetCategoriesForDropdown(vaka.VakaCategoryId);
            return View(vaka);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(VakaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Vaka/update-vaka", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            }
            model.Categories = await GetCategoriesForDropdown(model.VakaCategoryId);
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/vakalar");
            VakaViewModel vaka = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaViewModel>>>(json);
                vaka = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (vaka == null) return NotFound();
            return View(vaka);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Vaka/delete-vaka/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Vaka/vakalar");
            VakaViewModel vaka = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<VakaViewModel>>>(json);
                vaka = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (vaka == null) return NotFound();
            vaka.Categories = await GetCategoriesForDropdown(vaka.VakaCategoryId);
            return View(vaka);
        }


        [HttpPost]
        public async Task<ActionResult> UploadPreviewImage(Guid id, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(id.ToString()), "id");

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName); // "file" = API'de parametre adı

            await _httpClient.PostAsync($"{_apiBaseUrl}/Vaka/upload-image", content);
            
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePreviewImage([FromBody] VakaViewModel model)
        {
            if (model == null || model.Id == Guid.Empty)
                return Json(new { success = false });

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Vaka/delete-previewimage", new { vakaId = model.Id });
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }
    }
}
