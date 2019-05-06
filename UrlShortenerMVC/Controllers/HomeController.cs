using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Entities db = new Entities();

        public ActionResult SideNavbar()
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var clicks = db.Urls.Where(u => u.UserId == userId).Select(x => x.Clicks).ToList();
                var model = new HomeViewModel
                {
                    TotalClicks = clicks.Sum(),
                    TotalLinks = clicks.Count()
                };
                return PartialView("_SideNavbar", model);
            }
            return View("Error");
        }
    }
}