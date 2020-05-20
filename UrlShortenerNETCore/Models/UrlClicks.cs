using System;
using System.Collections.Generic;

namespace UrlShortenerNETCore.Models
{
    public partial class UrlClicks
    {
        public string Id { get; set; }
        public string UrlId { get; set; }
        public DateTime ClickedAt { get; set; }

        public virtual Urls Url { get; set; }
    }
}
