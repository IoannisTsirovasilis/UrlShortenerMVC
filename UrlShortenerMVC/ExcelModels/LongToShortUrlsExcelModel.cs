using Hangfire;
using OfficeOpenXml;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using UrlShortenerMVC.Jobs;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.ExcelModels
{
    public class LongToShortUrlsExcelModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public MemoryStream ExcelFileStream { get; set; }

        public static LongToShortUrlsExcelModel ExportLongToShortUrlsExcelFile(HttpPostedFileBase file, Entities db, HttpContextBase httpContext,
                                                                  string userId, string campaignId, string userIP,  ShortExcelViewModel shortExcelViewModel)
        {
            var model = new LongToShortUrlsExcelModel();
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string filePath = Path.Combine(httpContext.Server.MapPath("~/App_Data"),
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
                                        var token = UrlViewModel.GenerateLongToShortToken(db);
                                        var shortUrl = UrlViewModel.GenerateShortUrl(token);
                                        var isValidUrl = Uri.IsWellFormedUriString(ds.Tables[0].Rows[i][0].ToString(), UriKind.Absolute);
                                        if (!isValidUrl)
                                        {
                                            worksheet.Cells[i + 2, 1].Value = ds.Tables[0].Rows[i][0].ToString();
                                            worksheet.Cells[i + 2, 2].Value = "Invalid Url";
                                            continue;
                                        }
                                        var tmp = ds.Tables[0].Rows[i][0].ToString();
                                        var user = db.AspNetUsers.Find(userId);

                                        if (user == null)
                                        {
                                            File.Delete(filePath);
                                            throw new Exception();
                                        }
                                        campaignId = string.IsNullOrWhiteSpace(campaignId) ? "" : campaignId;
                                        var campaign = db.Campaigns.Find(campaignId);
                                        if (campaign != null && campaign.CreatedBy != user.Id)
                                        {
                                            File.Delete(filePath);
                                            throw new Exception();
                                        }
                                        var url = db.Urls.FirstOrDefault(x => x.LongUrl == tmp && x.UserId == user.Id && x.CampaignId == campaignId);
                                        if (url != null)
                                        {
                                            worksheet.Cells[i + 2, 1].Value = url.LongUrl;
                                            worksheet.Cells[i + 2, 2].Value = url.ShortUrl;
                                            continue;
                                        }
                                        if (UrlViewModel.HasReachedShorteningLimit(user.Id, null, db))
                                        {
                                            File.Delete(filePath);
                                            throw new Exception(WebConfigurationManager.AppSettings["MonthlyLimitReachedTitle"]);
                                        }
                                        worksheet.Cells[i + 2, 1].Value = ds.Tables[0].Rows[i][0].ToString();
                                        worksheet.Cells[i + 2, 2].Value = shortUrl;
                                        var newUrl = new UrlViewModel();
                                        do
                                        {
                                            newUrl.Id = Guid.NewGuid().ToString();
                                        } while (db.Urls.Find(newUrl.Id) != null);
                                        newUrl.Token = token;
                                        newUrl.ShortUrl = shortUrl;
                                        newUrl.LongUrl = ds.Tables[0].Rows[i][0].ToString();
                                        newUrl.UserId = userId;
                                        newUrl.Clicks = 0;
                                        newUrl.MaxClicks = shortExcelViewModel.MaxClicks;
                                        newUrl.Expires = shortExcelViewModel.Expires;
                                        newUrl.ExpiresAt = shortExcelViewModel.Expires ? DateTime.ParseExact(shortExcelViewModel.ExpiresAtString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?) null;
                                        newUrl.HasExpired = false;
                                        newUrl.IPAddress = userIP;
                                        newUrl.CreatedAt = DateTime.Now;
                                        if (campaign != null)
                                        {
                                            newUrl.CampaignId = campaign.Id;
                                        }
                                        if (newUrl.Expires)
                                        {
                                            var lifeSpan = (newUrl.ExpiresAt.Value.AddDays(1) - DateTime.Now).TotalSeconds;
                                            BackgroundJob.Schedule(() => JobScheduler.ExpireUrl(newUrl.Id), TimeSpan.FromSeconds(lifeSpan));
                                        }
                                        db.Urls.Add(newUrl);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    dbContextTransaction.Commit();
                    File.Delete(filePath);
                    model.Succeeded = true;
                    model.ExcelFileStream = new MemoryStream(package.GetAsByteArray());
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    model.Succeeded = false;
                    model.Message = e.Message;
                }
            }
            return model;
        }
    }
}