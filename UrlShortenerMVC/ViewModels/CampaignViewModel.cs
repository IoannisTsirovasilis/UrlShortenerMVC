using System;
using System.ComponentModel.DataAnnotations;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class CampaignViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(128, ErrorMessage = "The field {0} cannot exceed {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Starting Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public AspNetUser User { get; set; }

        public static implicit operator Campaign(CampaignViewModel model)
        {
            return new Campaign
            {
                Id = model.Id,
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy
            };
        }

        public static implicit operator CampaignViewModel(Campaign model)
        {
            return new CampaignViewModel
            {
                Id = model.Id,
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                User = model.AspNetUser
            };
        }
    }
}