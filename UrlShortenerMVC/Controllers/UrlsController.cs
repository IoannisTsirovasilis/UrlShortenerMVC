using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;
using Microsoft.AspNet.Identity;
using UrlShortenerMVC.ExcelModels;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class UrlsController : Controller
    {
        private Entities db = new Entities();       

        // GET: Urls
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var model = new List<UrlViewModel>();
            db.Urls.Where(u => u.UserId == userId).ToList().ForEach(delegate (Url u)
            {
                model.Add(u);
            });

            return View(model);
        }

        // GET: Urls/ShortExcel
        public ActionResult ShortExcel(string campaignId, string error)
        {
            var model = new ShortExcelViewModel { CampaignId = campaignId };
            model.Message = error;
            
            return View(model);
        }

        [HttpPost]
        public ActionResult ShortExcel(HttpPostedFileBase file, string campaignId)
        {
            if (file.ContentLength > 0)
            {
                var result = LongToShortUrlsExcelModel.ExportLongToShortUrlsExcelFile(file, db, HttpContext, User.Identity.GetUserId(), 
                                                                                      campaignId, Request.UserHostAddress);
                if (result.Succeeded)
                {
                    return File(result.ExcelFileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ShortenedLinks.xlsx");
                }                
                return RedirectToAction("ShortExcel", new { campaignId, error = result.Message });
            }
            return RedirectToAction("ShortExcel", new { campaignId, error = "Oops! Something went wrong. Please try again later." });
        }

        // GET: Urls/Create
        [AllowAnonymous]
        public ActionResult Create(string campaignId, string error)
        {
            ViewBag.Toast = string.IsNullOrWhiteSpace(error) ? "NoError" : error;
            var model = new UrlViewModel { CampaignId = campaignId };
            return View(model);
        }

        // POST: Urls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "LongUrl,CampaignId")] UrlViewModel model)
        {
            ViewBag.Toast = "NoError";
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

                        var user = db.AspNetUsers.Find(User.Identity.GetUserId());
                        if (user == null)
                        {
                            return RedirectToAction("Create", new { campaignId = model.CampaignId, error = "Error" });
                        }
                        // If url has already been shortened by this user, return it
                        var url = db.Urls.Where(x => x.LongUrl == model.LongUrl && x.UserId == user.Id && x.CampaignId == model.CampaignId).ToList();
                        if (url.Count > 0)
                        {
                            return View(new UrlViewModel { LongUrl = url.First().LongUrl, ShortUrl = url.First().ShortUrl, CampaignId = url.First().CampaignId });
                        }

                        model.Token = UrlViewModel.GenerateLongToShortToken(db);
                        model.ShortUrl = UrlViewModel.GenerateShortUrl(model.Token);
                        
                        if (User.Identity.IsAuthenticated) model.UserId = User.Identity.GetUserId();
                        model.Clicks = 0;
                        model.MaxClicks = 0;
                        model.Expires = false;
                        model.HasExpired = false;
                        model.IPAddress = Request.UserHostAddress;
                        model.CreatedAt = DateTime.Now;
                        var campaign = db.Campaigns.Find(model.CampaignId);
                        if (campaign != null && campaign.CreatedBy == User.Identity.GetUserId())
                        {
                            model.CampaignId = campaign.Id;
                        }
                        db.Urls.Add(model);
                        db.SaveChanges();

                        dbContextTransaction.Commit();                        
                        return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl, CampaignId = model.CampaignId });
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return RedirectToAction("Create", new { campaignId = model.CampaignId, error = "Error" });
                    }
                }                
            }
            return View(model);
        }

        public ActionResult SideNavbar()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                return View(db.Urls.Where(u => u.UserId == userId).ToList().Select(x => new UrlViewModel
                {
                    Id = x.Id,
                    ShortUrl = x.ShortUrl,
                    LongUrl = x.LongUrl,
                    Clicks = x.Clicks,
                    CreatedAt = x.CreatedAt,
                    Expires = x.Expires,
                    ExpiresAt = x.ExpiresAt,
                    HasExpired = x.HasExpired,
                    MaxClicks = x.MaxClicks
                }));
            }
            return View("Error");
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
