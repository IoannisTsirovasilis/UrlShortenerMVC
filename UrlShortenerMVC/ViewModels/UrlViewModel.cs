using System;
using System.ComponentModel.DataAnnotations;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class UrlViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "{0} is not valid.")]
        [MaxLength(512, ErrorMessage = "{0} cannot exceed {1} characters.")]
        [Url(ErrorMessage = "{0} is not valid.")]
        [Display(Name = "Link")]
        public string LongUrl { get; set; }

        public string ShortUrl { get; set; }
        public int Token { get; set; }
        public DateTime CreatedAt { get; set; }

        public static implicit operator Url(UrlViewModel model)
        {
            return new Url
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                Token = model.Token
            };
        }

        public static implicit operator UrlViewModel(Url model)
        {
            return new UrlViewModel
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                Token = model.Token
            };
        }
    }
}