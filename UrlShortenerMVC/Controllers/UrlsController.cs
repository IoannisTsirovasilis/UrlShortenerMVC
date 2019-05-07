﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;
using Microsoft.AspNet.Identity;
using UrlShortenerMVC.ExcelModels;
using System.Web.Configuration;
using PagedList;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class UrlsController : Controller
    {
        private Entities db = new Entities();       

        // GET: Urls
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            var model = new List<UrlViewModel>();
            try
            {
                var userId = User.Identity.GetUserId();
                var urls = db.Urls.Where(u => u.UserId == userId);

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                if (!string.IsNullOrEmpty(searchString))
                {
                    urls = from u in urls
                           where u.LongUrl.ToLower().Contains(searchString.ToLower()) 
                                 || u.ShortUrl.ToLower().Contains(searchString.ToLower())
                           select u;
                }

                urls.OrderByDescending(x => x.CreatedAt).ToList().ForEach(delegate (Url u)
                {
                    model.Add(u);
                });
            }
            catch (Exception)
            {
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        // GET: Urls/ShortExcel
        public ActionResult ShortExcel(string campaignId, string title, string message)
        {
            var model = new ShortExcelViewModel { CampaignId = campaignId };
            ViewBag.Title = title;
            ViewBag.Message = message;
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
            }
            return RedirectToAction("ShortExcel", new { campaignId, title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
        }

        // GET: Urls/Create
        [AllowAnonymous]
        public ActionResult Create(string campaignId, string title, string message)
        {            
            var model = new UrlViewModel { CampaignId = campaignId };
            ViewBag.Title = title;
            ViewBag.Message = message;
            return View(model);
        }

        // POST: Urls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create(UrlViewModel model)
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

                        Url url;
                        if (Request.IsAuthenticated)
                        {
                            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
                            if (user == null)
                            {
                                throw new Exception();
                            }
                            url = db.Urls.FirstOrDefault(x => x.LongUrl == model.LongUrl && x.UserId == user.Id && x.CampaignId == model.CampaignId);
                            if (url != null)
                            {
                                return View(new UrlViewModel { LongUrl = url.LongUrl, ShortUrl = url.ShortUrl, CampaignId = url.CampaignId });
                            }
                            model.UserId = user.Id;
                            var campaign = db.Campaigns.Find(model.CampaignId);
                            if (campaign != null && campaign.CreatedBy == user.Id)
                            {
                                model.CampaignId = campaign.Id;
                            }
                        }
                        else
                        {
                            url = db.Urls.FirstOrDefault(x => x.LongUrl == model.LongUrl && x.IPAddress == Request.UserHostAddress);
                            if (url != null)
                            {
                                return View(new UrlViewModel { LongUrl = url.LongUrl, ShortUrl = url.ShortUrl });
                            }
                        }
                        model.Token = UrlViewModel.GenerateLongToShortToken(db);
                        model.ShortUrl = UrlViewModel.GenerateShortUrl(model.Token);
                        
                        model.Clicks = 0;
                        model.MaxClicks = 0;
                        model.Expires = false;
                        model.HasExpired = false;
                        model.IPAddress = Request.UserHostAddress;
                        model.CreatedAt = DateTime.Now;
                       
                        db.Urls.Add(model);
                        db.SaveChanges();

                        dbContextTransaction.Commit();                        
                        return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl, CampaignId = model.CampaignId });
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        return RedirectToAction("Create", new { campaignId = model.CampaignId, title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
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
