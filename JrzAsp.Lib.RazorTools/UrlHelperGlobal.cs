using System.Web;
using System.Web.Mvc;

namespace JrzAsp.Lib.RazorTools {
    public static class UrlHelperGlobal {
        private static readonly string URL_HELPER_KEY = $"{typeof(UrlHelperGlobal)}:{nameof(URL_HELPER_KEY)}";
        public static UrlHelper Self {
            get {
                var uh = HttpContext.Current.Items[URL_HELPER_KEY];
                if (uh != null) return uh as UrlHelper;
                uh = new UrlHelper(HttpContext.Current.Request.RequestContext);
                HttpContext.Current.Items[URL_HELPER_KEY] = uh;
                return (UrlHelper) uh;
            }
        }
    }
}