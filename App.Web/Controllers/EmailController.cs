using App.Web.Models.Email;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
//using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class EmailController : ApiBaseController {

        private IEmailServices _service;
        private IMasterDataServices _setupServices;
        private IAppReminderJobServices _reminderSvc;
        public EmailController(IEmailServices service, IMasterDataServices setupSvc, IAppReminderJobServices reminderSvc) {
            _service = service;
            _setupServices = setupSvc;
            _reminderSvc = reminderSvc;
        }

        // Send Email
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SendEmail(string[] email)
        {
            string subject = "Example Subject Message";
            string body = "Example Body Message";

            ICSFileModel icsFileModel = new ICSFileModel() {
                Location = "Derivative C",
                Subject = subject,
                Description = "Schedule Decription",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            
            return _service.SendEmailICS(email, subject, body, icsFileModel);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage DistributeReminder([FromUri]string password) {

            if (password != WebAppSettings.DbMigrationSecret) {
                return new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
            
            _reminderSvc.DistributeReminder();
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SendEmailWithGuid(string[] email, string guid,int sequence,string method = "REQUEST") {
            string subject = "Example Subject Message";
            string body = "Example Body Message";

            ICSFileModel icsFileModel = new ICSFileModel() {
                Location = "Derivative C",
                Subject = subject,
                Description = "Schedule Decription",
                StartDate = DateTime.Parse("2019-10-06"),
                EndDate = DateTime.Parse("2019-10-06"),
                IcsGuid = guid ?? Guid.NewGuid().ToString(),
                Sequence = (sequence).ToString(),
                Method = method
            };

            return _service.SendEmailICS(email, subject, body, icsFileModel);
        }
    }
}