using App.DataAccess;
using App.Web.Services.Contracts;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Web.Utilities;
using App.DataAccess.Model;
using App.Web.Models.Email;

namespace App.Web.Services.Repositories {
    public class AppReminderJobServices : ServiceBase, IAppReminderJobServices {

        //private IAccountServices _accountServices;
        private IEmailServices _emailSvc;
        private string APP_DOMAIN => WebAppSettings.APPDomain;

        public AppReminderJobServices(ApplicationDbContext context, IEmailServices emailSvc) {
            this.context = context;
            this._emailSvc = emailSvc;
            //this._accountServices = accountServices;
        }

        public void StartWorker() {
            RecurringJob.AddOrUpdate("AppReminder", () => DistributeReminder(true),WebAppSettings.AppReminderJobSetting,TimeZoneInfo.Local);
        }

        public void DistributeReminder(bool sendCalendar = false) {

            var recipients = context.Users.Where(t => t.IsActive == true && t.IsDeleted != true && t.IsDraft == false && t.Email != null).Select(t => t.Email).ToArray();

            if (recipients.Length <= 0) return;

            var date = DateTime.Today.Date;

            var reminders = GetReminders(date);

            var subject = $"App Reminders {date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))}";

            var body = CreateBody(reminders);

            ICSFileModel icsFileModel = new ICSFileModel() {
                Location = "Location",
                Subject = subject,
                Description = subject,
                StartDate = date.ChangeTime(16, 0, 0, 0),
                EndDate = date.ChangeTime(8, 0, 0, 0),
                IcsGuid = Guid.NewGuid().ToString(),
                Sequence = 1.ToString(),
                Method = "REQUEST"
            };

            if(sendCalendar)
                _emailSvc.SendEmailICS(recipients, subject, body, icsFileModel);
            else
                _emailSvc.SendEmailOnly(recipients, subject, body);

            try {
                var log = new ActivityLog();

                log.Action = "Distribute Reminder";
                log.Url = "";
                log.IPAddress = "";
                log.HostName = "";
                log.Modul = "";
                log.Username = "SYSTEM";
                log.CreatedDate = DateTime.Now;
                log.Data = "Distribute Reminder " + DateTime.Now.ToString("yyyy MMM dd HH:mm:ss");

                context.ActivityLogs.Add(log);

                context.SaveChanges();
            } catch (Exception) {
                
            }
        }

        public string CreateBody(List<Flow> reminders) {

            var result = "<html><head><meta http-equiv=\"Content-Type\" content = \"text/html; charset=utf-8\" ></head><body>";

            if(reminders.Count > 0) {

                result += "<h3>Today's task</h3>";
                result += "</br></br></br>";

                var grouped = reminders.GroupBy(t => t.DisplayGrouping)
                    .OrderBy(t => t.Key != null ? (t.Key.StartsWith("Reminder_Equity") ? 0 : t.Key.StartsWith("Reminder_Fixed_Income") ? 1 : t.Key.StartsWith("Reminder_Derivative") ? 2 : 3) : 4)
                    .ToList();

                foreach (var group in grouped) {
                    result += $"<h4>{group.Key?.Replace("Reminder_","").Replace("_"," ")}</h4>";
                    result += "</br>";
                    result += "</br>";
                    result += "<ul>";

                    foreach (var item in group) {
                        result += $"<li style=\"list-style:disc inside;mso-special-format:bullet;\"><font style='color:black'><a href=\"{APP_DOMAIN}{item.Url}\">{item.Keterangan}</a></font></li>";
                    }

                    result += "</ul>";
                    result += "</br></br></br>";
                }


            } else {
                result = $"<h2>You have no task today<h2>";
            }

            result += "</body></html>";
            return result;
        }

        public List<Flow> GetReminders(DateTime date) {

            var tomorrow = date.Date.AddWorkDays(1);
            var data = context.Flows.Where(t =>
            t.IsDeleted != true
            && t.IsDraft == false
            &&
            (System.Data.Entity.DbFunctions.TruncateTime(t.DueDate) == date && t.DueDate < tomorrow)
            ).ToList();
            
            data = data
                .OrderBy(t => t.Keterangan)
                .ToList();

            return data;
        }

    }
}
