using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class HomeController : BaseController {
        private readonly ISysLogger _logger;
        private readonly IActivityLogServices _activityLog;
        private IHomeServices _services;

        public HomeController(ISysLogger logger,
            IActivityLogServices activityLog, 
            IHomeServices services) {
            _logger = logger;
            _activityLog = activityLog;
            _services = services;
        }
        
        public ActionResult Index() {
            //SetBreadCrumbs(new BreadCrumbItem("/home?id=123123", "Home 1"), new BreadCrumbItem("/Home/Detail?Id=33333", "Detail"));
            return View();
        }
        
        public ActionResult TestError() {
            _logger.LogMessage("Testing error... Wait for error...");
            throw new Exception("Test error!");
        }

        public ActionResult GetDoc(string id) {

            return File(_services.GetDoc(id));

        }
    }
}