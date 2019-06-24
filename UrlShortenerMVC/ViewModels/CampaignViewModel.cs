using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class CampaignViewModel
    {
        private readonly Entities db = new Entities();

        public string Id { get; set; }

        [Required]
        [MaxLength(128, ErrorMessage = "The field {0} cannot exceed {1} characters.")]
        public string Name { get; set; }


        [Display(Name = "Starting Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ending Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Starting Date")]
        public string StartDateString { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        public string EndDateString { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public AspNetUser User { get; set; }

        public bool IsActive { get; set; }

        public List<Url> Urls { get; }

        public int GetUrlsCount() => db.Urls.Where(x => x.CampaignId == Id).Count();

        public List<Url> GetUrls() => db.Urls.Where(x => x.CampaignId == Id).ToList();

        public static implicit operator Campaign(CampaignViewModel model)
        {
            return new Campaign
            {
                Id = model.Id,
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                IsActive = model.IsActive
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
                User = model.AspNetUser,
                StartDateString = model.StartDate.ToString("MM/dd/yyyy"),
                EndDateString = model.EndDate.ToString("MM/dd/yyyy"),
                IsActive = model.IsActive
            };
        }
    }
}