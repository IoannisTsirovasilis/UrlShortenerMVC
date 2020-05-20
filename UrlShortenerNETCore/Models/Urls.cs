using System;
using System.Collections.Generic;

namespace UrlShortenerNETCore.Models
{
    public partial class Urls
    {
        public Urls()
        {
            UrlClicks = new HashSet<UrlClicks>();
        }

        public string Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public string UserId { get; set; }
        public string CampaignId { get; set; }
        public string Ipaddress { get; set; }
        public int Token { get; set; }
        public int Clicks { get; set; }
        public int MaxClicks { get; set; }
        public bool Expires { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool HasExpired { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Campaigns Campaign { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<UrlClicks> UrlClicks { get; set; }
    }
}
