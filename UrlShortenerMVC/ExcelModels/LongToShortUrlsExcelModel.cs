using Elmah;
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
                    var ds = new DataSet();

                    //A 32-bit provider which enables the use of

                    var ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + @";Extended Properties=""Excel 12.0 Xml;HDR=Yes;IMEX=1;""";
                    var package = new ExcelPackage();

                    using (var conn = new OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (var dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            var sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            var query = "SELECT * FROM [" + sheetName + "]";
                            using (var command = new OleDbCommand(query, conn))
                            using (var adapter = new OleDbDataAdapter(command))
                            { adapter.Fill(ds); }
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    var user = db.AspNetUsers.Find(userId);

                                    if (user == null)
                                    {
                                        File.Delete(filePath);
                                        throw new Exception();
                                    }

                                    var campaign = db.Campaigns.Find(campaignId);

                                    if (campaign != null && campaign.CreatedBy != user.Id)
                                    {
                                        File.Delete(filePath);
                                        throw new Exception();
                                    }

                                    campaignId = campaign == null ? null : campaignId;

                                    var worksheet = package.Workbook.Worksheets.Add("New Sheet");
                                    worksheet.Cells[1, 1].Value = "Long";
                                    worksheet.Cells[1, 2].Value = "Short";
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        var longUrl = ds.Tables[0].Rows[i][0].ToString();
                                        var url = db.Urls.FirstOrDefault(x => x.LongUrl == longUrl && x.UserId == user.Id && x.CampaignId == campaignId);

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

                                        var isValidUrl = Uri.IsWellFormedUriString(longUrl, UriKind.Absolute);
                                        if (!isValidUrl)
                                        {
                                            worksheet.Cells[i + 2, 1].Value = longUrl;
                                            worksheet.Cells[i + 2, 2].Value = "Invalid Url";
                                            continue;
                                        }

                                        var token = UrlViewModel.GenerateLongToShortToken(db);
                                        var shortUrl = UrlViewModel.GenerateShortUrl(token);

                                        worksheet.Cells[i + 2, 1].Value = longUrl;
                                        worksheet.Cells[i + 2, 2].Value = shortUrl;

                                        var newUrl = new UrlViewModel();
                                        do
                                        {
                                            newUrl.Id = Guid.NewGuid().ToString();
                                        } while (db.Urls.Find(newUrl.Id) != null);
                                        newUrl.Token = token;
                                        newUrl.ShortUrl = shortUrl;
                                        newUrl.LongUrl = longUrl;
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
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbContextTransaction.Rollback();
                    model.Succeeded = false;
                    model.Message = ex.Message;
                }
            }
            return model;
        }
    }
}