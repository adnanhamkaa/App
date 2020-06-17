using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class NotificationController : BaseController {

        IWorkflowServices _wfSvc;

        public NotificationController(IWorkflowServices wfSvc) {
            this._wfSvc = wfSvc;
        }

        [HttpPost]
        public ActionResult GetTodos()
        {
            return Json(_wfSvc.GetToDoList());
        }


        [HttpPost]
        public ActionResult SetDone(string id) {
            return Json(_wfSvc.SetStatus(id,WorkflowForm.WF_STATUS_DONE));
        }
    }
}