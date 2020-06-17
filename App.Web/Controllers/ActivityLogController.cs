using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [Module(Name = AppModule.Activity_Log)]
    public class ActivityLogController : BaseController
    {
        IActivityLogServices _services;
        public ActivityLogController(IActivityLogServices services) {
            _services = services;
        }


        // GET: ActivityLog
        [AppAuth(Action = AppActions.Activity_Log)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AppAuth(Action = AppActions.Activity_Log)]
        public ActionResult List(DttRequestWithDate req) {
            var result = _services.GetList(req);
            return Json(result);
        }
    }
}