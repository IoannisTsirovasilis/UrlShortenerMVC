using System;
using System.ComponentModel.DataAnnotations;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class UrlsIPAddressViewModel
    {
        public string UrlId { get; set; }
        public string IPAddressId { get; set; }
        public string UserId { get; set; }

        public DateTime ClickedAt { get; set; }

        public static implicit operator UrlsIPAddress(UrlsIPAddressViewModel model)
        {
            return new UrlsIPAddress
            {
                UrlId = model.UrlId,
                IPAddressId = model.IPAddressId,
                ClickedAt = model.ClickedAt
            };
        }

        public static implicit operator UrlsIPAddressViewModel(UrlsIPAddress model)
        {
            return new UrlsIPAddressViewModel
            {
                UrlId = model.UrlId,
                IPAddressId = model.IPAddressId,
                ClickedAt = model.ClickedAt
            };
        }
    }
}