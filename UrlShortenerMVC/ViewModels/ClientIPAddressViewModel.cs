using System;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.ViewModels
{
    public class ClientIPAddressViewModel
    {
        public string Id { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        public static implicit operator ClientIPAddress(ClientIPAddressViewModel model)
        {
            return new ClientIPAddress
            {
                Id = model.Id,
                IPAddress = model.IPAddress,
                CreatedAt = model.CreatedAt
            };
        }

        public static implicit operator ClientIPAddressViewModel(ClientIPAddress model)
        {
            return new ClientIPAddressViewModel
            {
                Id = model.Id,
                IPAddress = model.IPAddress,
                CreatedAt = model.CreatedAt
            };
        }
    }
}