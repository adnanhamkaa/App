using App.Web.Utilities;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers {
    [OutputCache(Duration = 0, NoStore = true)]
    [RoutePrefix("ErrorPage")]
    public class ErrorPageController : BaseController {
        [Route("")]
        [HttpGet]
        public ActionResult Any() {
            Response.StatusCode = 500;
            var hei = BuildFauxErrorInfo(500);
            return View("Error", hei);
        }

        [Route("{statusCode}")]
        [HttpGet]
        public ActionResult Any(int statusCode) {
            Response.StatusCode = statusCode;
            var viewName = $"Error{statusCode}";
            var hei = BuildFauxErrorInfo(statusCode);
            if (System.IO.File.Exists(Server.MapPath($"~/Views/Shared/{viewName}.cshtml"))) {
                return View(viewName, hei);
            }

            return View("Error", hei);
        }
        
        private HandleErrorInfo BuildFauxErrorInfo(int statusCode = 500) {
            var hei = new HandleErrorInfo(new HttpException(statusCode, "Error page redirect by IIS."), "ErrorPage",
                "Any");
            return hei;
        }
    }
}