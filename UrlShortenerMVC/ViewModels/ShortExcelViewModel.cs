using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UrlShortenerMVC.ViewModels
{
    public class ShortExcelViewModel
    {
        [Required]
        [FileExtensions(Extensions = "xlsx|xls", ErrorMessage = "Please select an Excel file.")]
        public string File { get; set; }

        public string CampaignId { get; set; }
    }
}