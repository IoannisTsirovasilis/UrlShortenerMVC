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
    public class DashboardController : Controller
    {
        private Entities db = new Entities();
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult UrlsPartial()
        {
            var userId = User.Identity.GetUserId();

            var model = new List<UrlViewModel>();
            db.Urls.Where(u => u.UserId == userId).ToList().ForEach(delegate (Url u)
            {
                model.Add(u);
            });

            return View("_UrlsPartial", model);
        }

        [ChildActionOnly]
        public ActionResult CampaignsPartial()
        {
            var userId = User.Identity.GetUserId();
            var campaigns = db.Campaigns.Where(x => x.CreatedBy == userId).ToList().Select(x => new CampaignViewModel
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });

            return View("_CampaignsPartial", campaigns);
        }
    }    
}