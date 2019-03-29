using System;
using System.ComponentModel.DataAnnotations;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class UrlViewModel
    {
        private Entities db = new Entities();

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

        public ClientIPAddress IPAddress { get; set; }
        public string IPAddressId { get; set; }

        public int Token { get; set; }
        public int Clicks { get; set; }
        public int MaxClicks { get; set; }
        public bool Expires { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool HasExpired { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedAt { get; set; }

        public static implicit operator Url(UrlViewModel model)
        {
            return new Url
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                UserId = model.UserId,
                IPAddressId = model.IPAddressId,
                Token = model.Token,
                Clicks = model.Clicks,
                MaxClicks = model.MaxClicks,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                HasExpired = model.HasExpired,
                CreatedAt = model.CreatedAt

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
                IPAddressId = model.IPAddressId,
                IPAddress = model.ClientIPAddress,
                Token = model.Token,
                Clicks = model.Clicks,
                MaxClicks = model.MaxClicks,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                HasExpired = model.HasExpired,
                CreatedAt = model.CreatedAt
            };
        }
    }
}