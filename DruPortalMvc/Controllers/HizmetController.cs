using DruPortalMvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DruPortalMvc.Controllers
{
    public class HizmetController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public HizmetController(IConfiguration configuration)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Hizmet/hizmetler");
            List<HizmetViewModel> hizmetler = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<HizmetViewModel>>>(json);
                hizmetler = apiResponse?.Data ?? new();
            }

            return View(hizmetler);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Hizmet/" + id);
            HizmetViewModel hizmet = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<HizmetViewModel>>(json);
                hizmet = apiResponse?.Data;
            }

            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }


        [HttpPost]
        public async Task<IActionResult> Add(HizmetViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Hizmet/add-hizmet", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> AddNameOnly([FromBody] HizmetNameViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Hizmet/add-hizmet-name", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Edit( HizmetViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Hizmet/update-hizmet", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Hizmet/delete-hizmet/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<ActionResult> UploadPreviewImage(Guid id, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(id.ToString()), "id");

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName); // "file" = API'de parametre adı

            await _httpClient.PostAsync($"{_apiBaseUrl}/Hizmet/upload-image", content);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePreviewImage([FromBody] HizmetViewModel model)
        {
            if (model == null || model.Id == Guid.Empty)
                return Json(new { success = false });

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Hizmet/delete-previewimage", new { hizmetId = model.Id });
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> AddPreviewText([FromBody] HizmetPreviewTextViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Hizmet/add-previewText", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> AddIcon([FromBody] HizmetIconViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Hizmet/add-Icon", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            return Json(new { success = false });
        }
    }
} 