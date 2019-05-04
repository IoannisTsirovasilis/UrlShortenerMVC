using OfficeOpenXml;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class UrlViewModel
    {
        private Entities db = new Entities();
        private static readonly string base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string Id { get; set; }

        [Required(ErrorMessage = "{0} is not valid.")]
        [MaxLength(512, ErrorMessage = "{0} cannot exceed {1} characters.")]
        [Url(ErrorMessage = "{0} is not valid.")]
        [Display(Name = "Link")]
        public string LongUrl { get; set; }        

        [Display(Name = "Short Link")]
        public string ShortUrl { get; set; }

        public AspNetUser User { get; set; }
        public string UserId { get; set; }

        public string IPAddress { get; set; }

        public int Token { get; set; }
        public int Clicks { get; set; }

        [Display(Name = "Max Clicks")]
        public int MaxClicks { get; set; }
        public bool Expires { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? ExpiresAt { get; set; }

        [Display(Name ="Has Expired")]
        public bool HasExpired { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedAt { get; set; }

        public string CampaignId { get; set; }

        public Campaign Campaign { get; set; }

        public static int GenerateLongToShortToken(Entities db)
        {
            var r = new Random();
            int token;

            do
            {
                token = r.Next(0, int.MaxValue);
            } while (db.Urls.Count(u => u.Token == token) > 0);

            return token;
        }

        public static string GenerateShortUrl(int token)
        {
            var shortUrl = "http://85.73.244.104:6677/";
            do
            {
                shortUrl += base62[token % 62];
                token /= base62.Length;
            } while (token > 0);
            return shortUrl;
        }

        public static implicit operator Url(UrlViewModel model)
        {
            return new Url
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                UserId = model.UserId,
                IPAddress = model.IPAddress,
                Token = model.Token,
                Clicks = model.Clicks,
                MaxClicks = model.MaxClicks,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                HasExpired = model.HasExpired,
                CreatedAt = model.CreatedAt,
                CampaignId = model.CampaignId
            };
        }

        public static implicit operator UrlViewModel(Url model)
        {
            return new UrlViewModel
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                UserId = model.UserId,
                User = model.AspNetUser,
                IPAddress = model.IPAddress,
                Token = model.Token,
                Clicks = model.Clicks,
                MaxClicks = model.MaxClicks,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                HasExpired = model.HasExpired,
                CreatedAt = model.CreatedAt,
                CampaignId = model.CampaignId,
                Campaign = model.Campaign
            };
        }
    }
}