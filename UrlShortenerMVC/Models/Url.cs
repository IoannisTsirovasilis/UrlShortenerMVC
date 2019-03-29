//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UrlShortenerMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Url
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Url()
        {
            this.UrlsIPAddresses = new HashSet<UrlsIPAddress>();
        }
    
        public string Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public int Token { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int Clicks { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrlsIPAddress> UrlsIPAddresses { get; set; }
    }
}
