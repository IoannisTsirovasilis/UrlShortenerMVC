using System;
using System.Collections.Generic;

namespace UrlShortenerNETCore.Models
{
    public partial class Campaigns
    {
        public Campaigns()
        {
            Urls = new HashSet<Urls>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public virtual AspNetUsers CreatedByNavigation { get; set; }
        public virtual ICollection<Urls> Urls { get; set; }
    }
}
