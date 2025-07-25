using System;
using System.ComponentModel.DataAnnotations;

namespace DruPortalMvc.Models
{
    public class BlogCategoryViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
} 