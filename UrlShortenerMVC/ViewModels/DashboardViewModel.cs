using Chart.Mvc.ComplexChart;
using Chart.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlShortenerMVC.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalLinks { get; }
        public int TotalClicks { get; }
    }
}