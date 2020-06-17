using System.Web.Mvc;
using System.Web.Routing;

namespace App.Web {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "DefaultHomeIndex",
                "",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {
                    typeof(RouteConfig).Namespace + ".Controllers"
                }
            );

            routes.MapRoute(
                "DefaultIndex",
                "{controller}",
                new {action = "Index"},
                new[] {
                    typeof(RouteConfig).Namespace + ".Controllers"
                }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {
                    typeof(RouteConfig).Namespace + ".Controllers"
                }
            );
        }
    }
}