using Chart.Mvc.ComplexChart;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;
using System.Web.Configuration;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private Entities db = new Entities();
        // GET: Dashboard
        public ActionResult Index(string campaignId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                ViewBag.CampaignId = new SelectList(db.Campaigns.Where(x => x.CreatedBy == userId).OrderByDescending(x => x.CreatedAt), "Id", "Name", campaignId);

                var barChart = new BarChart();
                string[] labels;
                var values = new List<double>();
                var campaign = db.Campaigns.Find(campaignId);
                var today = DateTime.Now;
                var last = today.AddMonths(-1);

                if (campaign == null)
                {
                    labels = new string[(int)(DateTime.Now - DateTime.Now.AddMonths(-1)).TotalDays];

                    for (var i = 0; i < labels.Length; i++)
                    {
                        labels[i] = DateTime.Now.AddMonths(-1).AddDays(i).ToString("dd-MMM");
                        values.Add(0);
                    }
                }
                else
                {
                    labels = new string[(int)(campaign.EndDate - campaign.StartDate).TotalDays];

                    var totalClicks = db.UrlClicks.Include(x => x.Url).Where(x => x.Url.Campaign.Id == campaignId).ToList();
                    for (var i = 0; i < labels.Length; i++)
                    {
                        labels[i] = campaign.StartDate.AddDays(i).ToString("dd-MMM");
                        values.Add(totalClicks.Where(x => x.ClickedAt.Date == campaign.StartDate.AddDays(i).Date).Count());
                    }
                }

                barChart.ComplexData.Labels.AddRange(labels);
                barChart.ComplexData.Datasets = new List<ComplexDataset>
                {
                    new ComplexDataset
                    {
                        Data = values,
                        Label = campaign?.Name ?? "",
                        FillColor = "rgba(83,83,119,1)",
                        StrokeColor = "rgba(83,83,119,1)",
                        PointColor = "rgba(83,83,119,1)",
                        PointStrokeColor = "rgba(83,83,119,1)",
                        PointHighlightFill = "rgba(83,83,119,1)",
                        PointHighlightStroke = "rgba(83,83,119,1)"
                    }
                };
                ViewBag.BarChart = barChart;
                ViewBag.Campaign = campaignId;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
                return View();
            }
        }

        [ChildActionOnly]
        public ActionResult UrlsPartial(string campaignId)
        {
            

            var model = new List<UrlViewModel>();
            try
            {
                var userId = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(campaignId))
                {
                    db.Urls.Where(u => u.UserId == userId && u.CampaignId == campaignId).ToList().ForEach(delegate (Url u)
                    {
                        model.Add(u);
                    });
                }
                else
                {
                    db.Urls.Where(u => u.UserId == userId).ToList().ForEach(delegate (Url u)
                    {
                        model.Add(u);
                    });
                }                
            }
            catch (Exception)
            {
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            return View("_UrlsPartial", model);
        }

        [ChildActionOnly]
        public ActionResult CampaignsPartial()
        {
            var model = new List<CampaignViewModel>();
            try
            {
                var userId = User.Identity.GetUserId();
                db.Campaigns.Where(x => x.CreatedBy == userId).ToList().ForEach(delegate (Campaign c)
                {
                    model.Add(c);
                });

                
            }
            catch (Exception)
            {
                ViewBag.Title = WebConfigurationManager.AppSettings["ErrorTitle"];
                ViewBag.Message = WebConfigurationManager.AppSettings["ErrorMessage"];
            }
            return View("_CampaignsPartial", model);
        }
    }    
}