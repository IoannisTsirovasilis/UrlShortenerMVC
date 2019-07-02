using Elmah;
using Hangfire;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using UrlShortenerMVC.Jobs;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Entities db = new Entities();


        //[AllowAnonymous]
        //public ActionResult UnderConstruction()
        //{
        //    return View();
        //}

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

        [AllowAnonymous]
        public ActionResult Index(string title, string message)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Create", "Urls");
            }
            ViewBag.Title = title;
            ViewBag.Message = message;
            return View(new UrlViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UrlViewModel model)
        {
            if (ModelState.IsValid)
            {                
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        do
                        {
                            model.Id = Guid.NewGuid().ToString();
                        } while (db.Urls.Find(model.Id) != null);

                        var url = db.Urls.FirstOrDefault(x => x.LongUrl == model.LongUrl && x.UserId == null);
                        if (url != null)
                        {
                            return View(new UrlViewModel { LongUrl = url.LongUrl, ShortUrl = url.ShortUrl });
                        }

                        if (UrlViewModel.HasReachedShorteningLimit(null, Request.UserHostAddress, db))
                        {
                            return RedirectToAction("Create", new
                            {
                                campaignId = model.CampaignId,
                                title = WebConfigurationManager.AppSettings["MonthlyLimitReachedTitle"],
                                message = string.Format(WebConfigurationManager.AppSettings["MonthlyLimitReachedMessage"],
                                                                    WebConfigurationManager.AppSettings["MonthlyLimitUnauthenticated"],
                                                                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).ToString())
                            });
                        }

                        model.MaxClicks = 0;
                        model.Expires = true;
                        model.ExpiresAt = DateTime.Now.AddMonths(1);
                        model.Token = UrlViewModel.GenerateLongToShortToken(db);
                        model.ShortUrl = UrlViewModel.GenerateShortUrl(model.Token);

                        model.Clicks = 0;
                        model.HasExpired = false;
                        model.IPAddress = Request.UserHostAddress;
                        model.CreatedAt = DateTime.Now;

                        db.Urls.Add(model);
                        db.SaveChanges();

                        if (model.Expires)
                        {
                            var lifeSpan = (model.ExpiresAt.Value.AddDays(1) - DateTime.Now).TotalSeconds;
                            BackgroundJob.Schedule(() => JobScheduler.ExpireUrl(model.Id), TimeSpan.FromSeconds(lifeSpan));
                        }
                        dbContextTransaction.Commit();

                        return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl, CampaignId = model.CampaignId });
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbContextTransaction.Rollback();
                        return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
                    }
                }
            }
            return View(model);
        }
    }
}