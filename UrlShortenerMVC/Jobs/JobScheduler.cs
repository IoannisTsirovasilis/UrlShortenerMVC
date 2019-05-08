using Hangfire;
using System;
using System.Data.Entity;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.Jobs
{
    public class JobScheduler
    {
        private static Entities db = new Entities();

        public static void ExpireUrl(string id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var url = db.Urls.Find(id);
                    if (url == null)
                    {
                        return;
                    }
                    url.HasExpired = true;
                    db.Entry(url).State = EntityState.Modified;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    BackgroundJob.Schedule(() => ExpireUrl(id), TimeSpan.FromMinutes(5));
                }
            }
        }
    }
}