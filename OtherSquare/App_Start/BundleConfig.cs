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
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                "~/Scripts/OtherSquare/logger.js",
                "~/Scripts/OtherSquare/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/lms_fc").Include(
                "~/Scripts/OtherSquare/lms_flashcards.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                "~/Scripts/moment.js"
                ));

            #endregion

            #region Styles

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryUi").Include(
                "~/Content/themes/base/jquery-ui.css"));

            #endregion
        }
    }
}
