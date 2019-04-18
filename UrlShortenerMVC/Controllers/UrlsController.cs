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
                return View(db.Urls.Where(u => u.UserId == userId).ToList().Cast<UrlViewModel>());
            }
            return View("Error");
        }

        // GET: Urls/ShortExcel
        public ActionResult ShortExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShortExcel(HttpPostedFileBase file)
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

                                        db.Urls.Add(model);
                                    }    
                                }
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return File(new MemoryStream(package.GetAsByteArray()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Test.xlsx");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            ViewBag.Error = "Oops! Something went wrong.";
            return View();
        }

        // GET: Urls/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Urls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "LongUrl")] UrlViewModel model)
        {
            if (ModelState.IsValid)
            {
                do
                {
                    model.Id = Guid.NewGuid().ToString();
                } while (db.Urls.Find(model.Id) != null);

                model.Token = GenerateLongToShortToken();
                model.ShortUrl = ShortUrl(model.Token);
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

                db.Urls.Add(model);
                db.SaveChanges();
                
                return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl });
            }

            return View(model);
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
            var shortUrl = "http://192.168.1.2:6677/";
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
