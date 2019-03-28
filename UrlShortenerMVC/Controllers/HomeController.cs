using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel model)
        {
            var Random = new Random();
            var random = Random.Next(0, int.MaxValue);
            var enc = "http://short.it/";
            do
            {
                enc += base62[random % 62];
                random /= 62;
            } while (random > 0);
            model.ShortUrl = enc;
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}