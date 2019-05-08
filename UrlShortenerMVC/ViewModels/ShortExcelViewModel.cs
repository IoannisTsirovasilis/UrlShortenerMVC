using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortenerMVC.ViewModels
{
    public class ShortExcelViewModel
    {
        [Required]
        [FileExtensions(Extensions = "xlsx|xls", ErrorMessage = "Please select an Excel file.")]
        public string File { get; set; }

        public string CampaignId { get; set; }

        public string Message { get; set; }

        [Display(Name = "Expiration Date")]
        public string ExpiresAtString { get; set; }

        [Range(0, 10000, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name = "Max Clicks")]
        public int MaxClicks { get; set; }
        public bool Expires { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? ExpiresAt { get; set; }
    }
}