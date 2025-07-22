using System;

namespace DruPortalMvc.Models
{
    public class HizmetViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string PreviewImageUrl { get; set; }

        public string PreviewText { get; set; }

        public string Icon { get; set; }
    }
} 