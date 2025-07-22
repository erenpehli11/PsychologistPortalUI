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
    public class BlogController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BlogController(IConfiguration configuration)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5014/api";
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/blogs");
            List<BlogPostViewModel> blogs = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogPostViewModel>>>(json);
                blogs = apiResponse?.Data ?? new();
            }

            return View(blogs);
        }

        private async Task<List<SelectListItem>> GetCategoriesForDropdown(Guid? selectedId = null)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/categories");
            List<BlogCategoryViewModel> categories = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogCategoryViewModel>>>(json);
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
            var model = new BlogPostViewModel
            {
                Categories = await GetCategoriesForDropdown()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {

                
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Blog/add-blog", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            }
            model.Categories = await GetCategoriesForDropdown();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/blogs");
            BlogPostViewModel blog = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogPostViewModel>>>(json);
                blog = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (blog == null) return NotFound();

            blog.Categories = await GetCategoriesForDropdown(blog.BlogCategoryId);
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {
               
              
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Blog/update-blog", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            }
            model.Categories = await GetCategoriesForDropdown(model.BlogCategoryId);
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/blogs");
            BlogPostViewModel blog = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogPostViewModel>>>(json);
                blog = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (blog == null) return NotFound();

            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Blog/delete-blog/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "API hatası: " + response.ReasonPhrase);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Blog/blogs");
            BlogPostViewModel blog = null;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<BlogPostViewModel>>>(json);
                blog = apiResponse?.Data?.Find(x => x.Id == id);
            }

            if (blog == null) return NotFound();

            blog.Categories = await GetCategoriesForDropdown(blog.BlogCategoryId);
            return View(blog);
        }

        [HttpPost]
        public async Task<ActionResult> UploadPreviewImage(Guid id, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(id.ToString()), "id");

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName); // "file" = API'de parametre adı

            await _httpClient.PostAsync($"{_apiBaseUrl}/Blog/upload-image", content);
            

            return Ok();


        }

            [HttpPost]
        public async Task<IActionResult> DeletePreviewImage([FromBody] BlogPostViewModel model)
        {
            if (model == null || model.Id == Guid.Empty)
                return Json(new { success = false });

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Blog/delete-previewimage", new { blogId = model.Id });
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false });
        }
    }
} 