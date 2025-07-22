using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DruPortalMvc.Models
{
    public class VakaViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? PreviewImageUrl { get; set; }
        public string? ShortDescription { get; set; }
        public Guid VakaCategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public IFormFile? PreviewImageFile { get; set; }
        public List<string>? ContentImageUrls { get; set; }
        public DateTime CreatedAt { get; set; }

    }
} 