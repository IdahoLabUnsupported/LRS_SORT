using System.Net;
using Inl.MvcHelper;
using StackExchange.Profiling;
using Sort.Business;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Sort.Mvc.App_Start;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.IdentityModel.Logging;

namespace Sort.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ConfigureViewEngines();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            MiniProfiler.Settings.PopupRenderPosition = RenderPosition.BottomRight;
            Config.SetDbConfigVals();

            // changing default helper path for chose css
            BsHelper.SetDependencyPrimaryCss(Dependency.Chosen, "~/Content/bootstrap-chosen.css");
            ConfigureAntiForgeryTokens();
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            IdentityModelEventSource.ShowPII = true;
        }

        protected void Application_BeginRequest()
        {
            if (Config.ExecutionMode == ExecutionMode.Local)
            {
                MiniProfiler.Start();
            }
            else
            {
                HttpsOffLoaded();
            }

        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        /// <summary>
        /// Configures the view engines. By default, Asp.Net MVC includes the Web Forms (WebFormsViewEngine) and 
        /// Razor (RazorViewEngine) view engines. You can remove view engines you are not using here for better
        /// performance.
        /// </summary>
        private static void ConfigureViewEngines()
        {
            // Only use the RazorViewEngine.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        /// <summary>
        /// Configures the anti-forgery tokens. See 
        /// http://www.asp.net/mvc/overview/security/xsrfcsrf-prevention-in-aspnet-mvc-and-web-pages
        /// </summary>
        private static void ConfigureAntiForgeryTokens()
        {
            // Rename the Anti-Forgery cookie from "__RequestVerificationToken" to "f". This adds a little security 
            // through obscurity and also saves sending a few characters over the wire. Sadly there is no way to change 
            // the form input name which is hard coded in the @Html.AntiForgeryToken helper and the 
            // ValidationAntiforgeryTokenAttribute to  __RequestVerificationToken.
            // <input name="__RequestVerificationToken" type="hidden" value="..." />
            AntiForgeryConfig.CookieName = "f";

            // If you have enabled SSL. Uncomment this line to ensure that the Anti-Forgery 
            // cookie requires SSL to be sent across the wire. 
            AntiForgeryConfig.RequireSsl = false;

            // Set the UniqueClaimTypeIdentifier so the antiforgery token piece doesn't crash
            AntiForgeryConfig.UniqueClaimTypeIdentifier = System.IdentityModel.Claims.ClaimTypes.Name;
        }

        /// <summary>
        /// Will mark the request as secure if using http scheme which will be the case on servers where SSL is terminated on a reverse proxy/load balancer
        /// </summary>
        private static void HttpsOffLoaded()
        {
            var headers = HttpContext.Current.Request.Headers;
            var serverVariables = HttpContext.Current.Request.ServerVariables;
            if (headers.Get("X-Forwarded-Proto") != "https" || serverVariables.Get("HTTPS") != "on")
            {
                serverVariables.Set("HTTPS", "on");
                serverVariables.Set("SERVER_PORT", "443");
                serverVariables.Set("SERVER_PORT_SECURE", "1");
                headers.Set("X-Forwarded-Proto", "https");
            }
        }

    }
}
