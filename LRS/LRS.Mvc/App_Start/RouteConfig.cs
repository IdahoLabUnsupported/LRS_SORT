using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LRS.Mvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            // IgnoreRoute - Tell the routing system to ignore certain routes for better performance.
            // Ignore .axd files.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Ignore everything in the Content folder.
            routes.IgnoreRoute("Content/{*pathInfo}");
            // Ignore everything in the Scripts folder.
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            // Ignore the Forbidden.html file.
            routes.IgnoreRoute("Error/Forbidden.html");
            // Ignore the GatewayTimeout.html file.
            routes.IgnoreRoute("Error/GatewayTimeout.html");
            // Ignore the ServiceUnavailable.html file.
            routes.IgnoreRoute("Error/ServiceUnavailable.html");
            // Ignore the humans.txt file.
            routes.IgnoreRoute("humans.txt");

            // Uncomment to Enable attribute routing.
            // routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "MvcHelper",
                url: "MvcHelper/{action}/{file}",
                defaults: new { controller = "Helper" },
                namespaces: new string[] { "Inl.MvcHelper.Controllers" });

            routes.MapRoute(
                name: "EmployeeHelper",
                url: "EmployeeHelper/{action}/{file}",
                defaults: new { controller = "Helper" },
                namespaces: new string[] { "LRS.EmployeeHelper.Controllers" });

            routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
