using System.Web;
using System.Web.Optimization;

namespace UrlShortenerMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/short-excel").Include(
                         "~/Scripts/Custom/short-excel.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker").Include(
                         "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker-init").Include(
                         "~/Scripts/Custom/datepicker-init.js"));

            bundles.Add(new ScriptBundle("~/bundles/tooltip-init").Include(
                         "~/Scripts/Custom/tooltip-init.js"));

            bundles.Add(new ScriptBundle("~/bundles/toast-init").Include(
                         "~/Scripts/Custom/toast-init.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/Custom/styles.css",
                      "~/Content/bootstrap-datepicker3.standalone.css"));
        }
    }
}
