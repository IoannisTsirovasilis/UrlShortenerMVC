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
    }
}