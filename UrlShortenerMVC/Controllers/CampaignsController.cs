using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
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
        public ActionResult Index(string error)
        {
            ViewBag.Toast = string.IsNullOrWhiteSpace(error) ? "NoError" : error;
            var userId = User.Identity.GetUserId();
            var campaigns = db.Campaigns.Where(x => x.CreatedBy == userId).ToList().Select(x => new CampaignViewModel
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });
            return View(campaigns);
        }

        // GET: Campaigns/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            return View((CampaignViewModel) campaign);
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
        public ActionResult Create([Bind(Include = "Name,StartDate,EndDate")] CampaignViewModel campaign)
        {
            if (campaign.StartDate > campaign.EndDate)
            {
                ModelState.AddModelError("StartDateGreaterThanEndDate", "Starting Date cannot be greater than Ending Date.");
                return View(campaign);
            }
            if (ModelState.IsValid)
            {
                do
                {
                    campaign.Id = Guid.NewGuid().ToString();
                } while (db.Campaigns.Find(campaign.Id) != null);
                campaign.CreatedAt = DateTime.Now;
                campaign.CreatedBy = User.Identity.GetUserId();
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            return View((CampaignViewModel) campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartDate,EndDate")] CampaignViewModel model)
        {
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("StartDateGreaterThanEndDate", "Starting Date cannot be greater than Ending Date.");
                return View(model);
            }            
            if (ModelState.IsValid)
            {
                var campaign = db.Campaigns.Find(model.Id);
                if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
                {
                    return RedirectToAction("Index", new { error = "Error" });
                }
                campaign.Name = model.Name;
                campaign.StartDate = model.StartDate;
                campaign.EndDate = model.EndDate;
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Campaigns/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null || campaign.CreatedBy != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", new { error = "Error" });
            }
            db.Campaigns.Remove(campaign);
            db.SaveChanges();
            return RedirectToAction("Index");
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
