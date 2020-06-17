using System.Web.Mvc;

namespace App.Web.Utilities {
    public static class AreaRegistrationExtensions {
        public static void RegisterDefaultAreaRoute(this AreaRegistration reg, AreaRegistrationContext context) {
            context.MapRoute(
                $"{reg.AreaName}_DefaultHomeIndex",
                $"{reg.AreaName}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {
                    reg.GetType().Namespace + ".Controllers"
                }
            );

            context.MapRoute(
                $"{reg.AreaName}_DefaultIndex",
                $"{reg.AreaName}/{{controller}}",
                new {action = "Index"},
                new[] {
                    reg.GetType().Namespace + ".Controllers"
                }
            );

            context.MapRoute(
                $"{reg.AreaName}_Default",
                $"{reg.AreaName}/{{controller}}/{{action}}/{{id}}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {
                    reg.GetType().Namespace + ".Controllers"
                }
            );
        }
    }
}