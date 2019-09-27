using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OtherSquare
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    /// <summary>
    /// All the AppSettings in the Web.config file can be accessed here.
    /// </summary>
    public static class AppSettings
    {


        /// <summary>If a setting from the AppSettings returns an unexpected value, log it.</summary>
        private static void Error()
        {
            //TODO add logging
        }
    }
}
