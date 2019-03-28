﻿using System;
using System.ComponentModel.DataAnnotations;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class UrlViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(128, ErrorMessage = "{0} cannot exceed {1} characters.")]
        // [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$", ErrorMessage = "Please enter a valid Url.")]
        [Display(Name = "Enter Url")]
        public string LongUrl { get; set; }

        public string ShortUrl { get; set; }
        public int Key { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Expires { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int MaxClicks { get; set; }
        public int CurrentClicks { get; set; }
        public bool IsActive { get; set; }

        public static implicit operator Url(UrlViewModel model)
        {
            return new Url
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                Key = model.Key,
                CreatedAt = model.CreatedAt,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                MaxClicks = model.MaxClicks,
                CurrentClicks = model.CurrentClicks,
                IsActive = model.IsActive
            };
        }

        public static implicit operator UrlViewModel(Url model)
        {
            return new UrlViewModel
            {
                Id = model.Id,
                LongUrl = model.LongUrl,
                ShortUrl = model.ShortUrl,
                Key = model.Key,
                CreatedAt = model.CreatedAt,
                Expires = model.Expires,
                ExpiresAt = model.ExpiresAt,
                MaxClicks = model.MaxClicks,
                CurrentClicks = model.CurrentClicks,
                IsActive = model.IsActive
            };
        }
    }
}