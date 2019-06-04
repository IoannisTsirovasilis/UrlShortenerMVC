using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Configuration;
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
                var today = DateTime.Now;
                var firstDateOfMonth = new DateTime(today.Year, today.Month, 1);
                var firstDateOfNextMonth = firstDateOfMonth.AddMonths(1);
                var userId = User.Identity.GetUserId();
                var clicks = db.Urls.Where(u => u.UserId == userId).Select(x => x.Clicks).ToList();
                var monthlyLinks = db.Urls.Where(u => u.UserId == userId && u.CreatedAt >= firstDateOfMonth && u.CreatedAt < firstDateOfNextMonth).Count();
                var model = new HomeViewModel
                {
                    TotalClicks = clicks.Sum(),
                    TotalLinks = clicks.Count(),
                    RemainingLinks = int.Parse(WebConfigurationManager.AppSettings["MonthlyLimitAuthenticated"]) - monthlyLinks > 0 ? 
                                     int.Parse(WebConfigurationManager.AppSettings["MonthlyLimitAuthenticated"]) - monthlyLinks : 0
                };
                return PartialView("_SideNavbar", model);
            }
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}