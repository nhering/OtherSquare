using System.Web;
using System.Web.Optimization;

namespace OtherSquare
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //    "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                "~/Scripts/OtherSquare/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/study").Include(
                "~/Scripts/OtherSquare/study.js"));

            #endregion

            #region Styles

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            #endregion
        }
    }
}
