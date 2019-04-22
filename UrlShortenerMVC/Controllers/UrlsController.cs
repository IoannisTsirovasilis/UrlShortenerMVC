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
using Microsoft.AspNet.Identity;
using System.IO;
using System.Data.OleDb;
using OfficeOpenXml;

namespace UrlShortenerMVC.Controllers
{
    [Authorize]
    public class UrlsController : Controller
    {
        private Entities db = new Entities();
        private readonly string base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // GET: Urls
        public ActionResult Index()
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

        // GET: Urls/ShortExcel
        public ActionResult ShortExcel(string campaignId, string error)
        {
            ViewBag.Toast = string.IsNullOrWhiteSpace(error) ? "NoError" : error;
            var model = new ShortExcelViewModel { CampaignId = campaignId };
            return View(model);
        }

        [HttpPost]
        public ActionResult ShortExcel(HttpPostedFileBase file, string campaignId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (file.ContentLength > 0)
                    {
                        string filePath = Path.Combine(HttpContext.Server.MapPath("~/TempFiles"),
                        Path.GetFileName(file.FileName));
                        file.SaveAs(filePath);
                        DataSet ds = new DataSet();

                        //A 32-bit provider which enables the use of

                        string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + @";Extended Properties=""Excel 12.0 Xml;HDR=Yes;IMEX=1;""";
                        var package = new ExcelPackage();

                        using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                        {
                            conn.Open();
                            using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                            {
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                string query = "SELECT * FROM [" + sheetName + "]";
                                using (OleDbCommand command = new OleDbCommand(query, conn))
                                using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                                { adapter.Fill(ds); }
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        var worksheet = package.Workbook.Worksheets.Add("New Sheet");
                                        worksheet.Cells[1, 1].Value = "Long";
                                        worksheet.Cells[1, 2].Value = "Short";
                                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                        {
                                            var token = GenerateLongToShortToken();
                                            var shortUrl = ShortUrl(token);
                                            var isValidUrl = Uri.IsWellFormedUriString(ds.Tables[0].Rows[i][0].ToString(), UriKind.Absolute);
                                            if (!isValidUrl)
                                            {
                                                worksheet.Cells[i + 2, 1].Value = ds.Tables[0].Rows[i][0].ToString();
                                                worksheet.Cells[i + 2, 2].Value = "Invalid Url";
                                                continue;
                                            }
                                            var tmp = ds.Tables[0].Rows[i][0].ToString();
                                            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
                                            
                                            if (user == null)
                                            {
                                                return RedirectToAction("ShortExcel", new { campaignId, error = "Error" });
                                            }
                                            campaignId = string.IsNullOrWhiteSpace(campaignId) ? null : campaignId;
                                            var campaign = db.Campaigns.Find(campaignId);
                                            if (campaign != null && campaign.CreatedBy != user.Id)
                                            {
                                                return RedirectToAction("ShortExcel", new { campaignId, error = "Error" });
                                            }
                                            var url = db.Urls.Where(x => x.LongUrl == tmp && x.UserId == user.Id && x.CampaignId == campaignId).ToList();
                                            if (url.Count > 0)
                                            {
                                                worksheet.Cells[i + 2, 1].Value = url.First().LongUrl;
                                                worksheet.Cells[i + 2, 2].Value = url.First().ShortUrl;
                                                continue;
                                            }
                                            worksheet.Cells[i + 2, 1].Value = ds.Tables[0].Rows[i][0].ToString();
                                            worksheet.Cells[i + 2, 2].Value = shortUrl;
                                            var model = new UrlViewModel();
                                            do
                                            {
                                                model.Id = Guid.NewGuid().ToString();
                                            } while (db.Urls.Find(model.Id) != null);
                                            model.Token = token;
                                            model.ShortUrl = shortUrl;
                                            model.LongUrl = ds.Tables[0].Rows[i][0].ToString();
                                            var userIP = db.ClientIPAddresses.Where(x => x.IPAddress == Request.UserHostAddress).ToList();
                                            if (userIP.Count == 0)
                                            {
                                                var ipAddress = new ClientIPAddress();
                                                do
                                                {
                                                    ipAddress.Id = Guid.NewGuid().ToString();
                                                } while (db.ClientIPAddresses.Find(ipAddress.Id) != null);
                                                ipAddress.IPAddress = Request.UserHostAddress;
                                                ipAddress.CreatedAt = DateTime.Now;
                                                db.ClientIPAddresses.Add(ipAddress);
                                                db.SaveChanges();
                                            }
                                            if (User.Identity.IsAuthenticated) model.UserId = User.Identity.GetUserId();
                                            model.Clicks = 0;
                                            model.MaxClicks = 0;
                                            model.Expires = false;
                                            model.HasExpired = false;
                                            model.IPAddressId = db.ClientIPAddresses.Where(x => x.IPAddress == Request.UserHostAddress).First().Id;
                                            model.CreatedAt = DateTime.Now;
                                            if (campaign != null) model.CampaignId = campaign.Id;

                                            db.Urls.Add(model);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        System.IO.File.Delete(filePath);
                        return File(new MemoryStream(package.GetAsByteArray()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Test.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("ShortExcel", new { campaignId, error = "Error" });
                }
            }
            return RedirectToAction("ShortExcel", new { campaignId, error = "Error" });
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

                        var userIP = db.ClientIPAddresses.Where(x => x.IPAddress == Request.UserHostAddress).ToList();
                        if (userIP.Count == 0)
                        {
                            var ipAddress = new ClientIPAddress();
                            do
                            {
                                ipAddress.Id = Guid.NewGuid().ToString();
                            } while (db.ClientIPAddresses.Find(ipAddress.Id) != null);
                            ipAddress.IPAddress = Request.UserHostAddress;
                            ipAddress.CreatedAt = DateTime.Now;
                            db.ClientIPAddresses.Add(ipAddress);
                            db.SaveChanges();
                        }

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

                        model.Token = GenerateLongToShortToken();
                        model.ShortUrl = ShortUrl(model.Token);
                        
                        if (User.Identity.IsAuthenticated) model.UserId = User.Identity.GetUserId();
                        model.Clicks = 0;
                        model.MaxClicks = 0;
                        model.Expires = false;
                        model.HasExpired = false;
                        model.IPAddressId = db.ClientIPAddresses.Where(x => x.IPAddress == Request.UserHostAddress).First().Id;
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

        private int GenerateLongToShortToken()
        {
            var r = new Random();
            int token;

            do
            {
                token = r.Next(0, int.MaxValue);
            } while (db.Urls.Count(u => u.Token == token) > 0);

            return token;
        }

        private string ShortUrl(int token)
        {
            var shortUrl = "http://2.87.93.82:6677/";
            do
            {
                shortUrl += base62[token % 62];
                token /= base62.Length;
            } while (token > 0);
            return shortUrl;
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
