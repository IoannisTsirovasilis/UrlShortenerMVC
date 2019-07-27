using Elmah;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class CampaignsController : Controller
    {
        private Entities db = new Entities();

        // GET: Campaigns
        public ActionResult Index(string title, string message)
        {
            var model = new List<CampaignViewModel>();
            try
            {
                var userId = User.Identity.GetUserId();
                db.Campaigns.Where(x => x.CreatedBy == userId && x.IsActive).ToList().ForEach(delegate (Campaign c)
                {
                    model.Add(c);
                });                
                ViewBag.Title = title;
                ViewBag.Message = message;                
            }
            catch (Exception)
            {
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            return View(model);
        }

        // GET: Campaigns/Details/5
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception();
                }
                CampaignViewModel campaign = db.Campaigns.Find(id);
                if (campaign == null)
                {
                    throw new Exception("Campaign does not exist.");
                }

                if (campaign.CreatedBy != User.Identity.GetUserId())
                {
                    throw new Exception("User does not own this campaign.");
                }

                return View(campaign);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
            }
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CampaignViewModel campaign)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    if (db.Campaigns.FirstOrDefault(x => x.Name == campaign.Name && x.CreatedBy == userId) != null)
                    {
                        ModelState.AddModelError("CampaignExists", "A campaign with this name already exists.");
                        return View(campaign);
                    }

                    campaign.StartDate = DateTime.ParseExact(campaign.StartDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    campaign.EndDate = DateTime.ParseExact(campaign.EndDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (campaign.StartDate > campaign.EndDate)
                    {
                        ModelState.AddModelError("StartDateGreaterThanEndDate", "Starting Date cannot be greater than Ending Date.");
                        return View(campaign);
                    }
                    do
                    {
                        campaign.Id = Guid.NewGuid().ToString();
                    } while (db.Campaigns.Find(campaign.Id) != null);
                    campaign.CreatedAt = DateTime.Now;
                    campaign.CreatedBy = userId;
                    campaign.IsActive = true;
                    db.Campaigns.Add(campaign);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (FormatException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("InvalidFormat", "Invalid Date Format");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception();
                }
                CampaignViewModel campaign = db.Campaigns.Find(id);
                if (campaign == null)
                {
                    throw new Exception("Campaign does not exist.");
                }

                if (campaign.CreatedBy != User.Identity.GetUserId())
                {
                    throw new Exception("User does not own this campaign.");
                }
                return View(campaign);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
            }
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CampaignViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    if (db.Campaigns.FirstOrDefault(x => x.Name == model.Name && x.Id != model.Id && x.CreatedBy == userId) != null)
                    {
                        ModelState.AddModelError("CampaignExists", "A campaign with this name already exists.");
                        return View(model);
                    }

                    model.StartDate = DateTime.ParseExact(model.StartDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    model.EndDate = DateTime.ParseExact(model.EndDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (model.StartDate > model.EndDate)
                    {
                        ModelState.AddModelError("StartDateGreaterThanEndDate", "Starting Date cannot be greater than Ending Date.");
                        return View(model);
                    }
                    var campaign = db.Campaigns.Find(model.Id);
                    if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
                    {
                        return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
                    }
                    campaign.Name = model.Name;
                    campaign.StartDate = model.StartDate;
                    campaign.EndDate = model.EndDate;
                    db.Entry(campaign).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (FormatException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("InvalidFormat", "Invalid Date Format");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            return View(model);
        }

        // GET: Campaigns/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception();
                }
                CampaignViewModel campaign = db.Campaigns.Find(id);
                if (campaign == null)
                {
                    throw new Exception("Campaign does not exist.");
                }

                if (campaign.CreatedBy != User.Identity.GetUserId())
                {
                    throw new Exception("User does not own this campaign.");
                }
                return View(campaign);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
            }
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new Exception();
                    }
                    Campaign campaign = db.Campaigns.Find(id);
                    if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
                    {
                        throw new Exception();
                    }
                    var urls = db.Urls.Where(x => x.CampaignId == campaign.Id).ToList();
                    urls.ForEach(delegate (Url x)
                    {
                        x.HasExpired = true;
                        x.ExpiresAt = DateTime.Now;
                        x.Expires = true;
                        db.Entry(x).State = EntityState.Modified;
                    });

                    campaign.IsActive = false;
                    db.Entry(campaign).State = EntityState.Modified;

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbContextTransaction.Rollback();
                    return RedirectToAction("Index", new { title = WebConfigurationManager.AppSettings["ErrorTitle"], message = WebConfigurationManager.AppSettings["ErrorMessage"] });
                }
            }                      
        }

        // GET: Urls/AddLinks/campaignId
        public ActionResult AddLinks(string campaignId)
        {
            ViewBag.CampaignId = campaignId;
            return View();
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
