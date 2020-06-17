using System.Web.Mvc;

namespace App.Web {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new Utilities.XssFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}