using App.Web.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Services.Contracts {
    public interface IEmailServices : IServiceBase {
        bool SendEmail(string[] emailDestination, string subject, string body);
        bool SendEmailICS(string[] emailDestination, string subject, string body, ICSFileModel icsFileModel);
        bool SendEmailOnly(string[] emailDestination, string subject, string body);
    }
}