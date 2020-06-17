using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class GeneralController : BaseController
    {
        // GET: General

        public GeneralController() {
        }

        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "module")]
        public ActionResult GetHolidays(string module) {

            return Json(new List<dynamic>());
        }
    }
}