using System;
using System.Collections.Generic;

namespace UrlShortenerNETCore.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            Campaigns = new HashSet<Campaigns>();
            Urls = new HashSet<Urls>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Campaigns> Campaigns { get; set; }
        public virtual ICollection<Urls> Urls { get; set; }
    }
}
