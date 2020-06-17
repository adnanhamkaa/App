using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using Newtonsoft.Json;
using App.DataAccess.Model;
using App.DataAccess;
using AutoMapper;

namespace App.Web.Services.Repositories {
    public class ActivityLogAttribute : ActionFilterAttribute {

        public string Modul { get; set; }
        public string Action { get; set; }


        public void InsertLog(ActivityLogModel activityLogModel) {
            using (ApplicationDbContext context = new ApplicationDbContext()) {
                var log = Mapper.Map<ActivityLog>(activityLogModel);
                log.Init();
                log.SetCreated();
                context.ActivityLogs.Add(log);
                context.SaveChanges();
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext) {

            base.OnActionExecuted(filterContext);
            
            var log = new ActivityLogModel() {
                IPAddress = HttpContext.Current.Request.UserHostAddress,
                HostName = HttpContext.Current.Request.UserHostAddress,
                Username = HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : null,
                Modul = this.Modul,
                Action = this.Action,
                CreatedDate = DateTime.Now
            };

            var form = filterContext.HttpContext.Request.Form;

            if (form != null) {
                try {

                    var dictionary = form.AllKeys.ToDictionary(k => k, k => form[k]);

                    var jsonPostedData = JsonConvert.SerializeObject(dictionary);

                    log.Data = jsonPostedData;
                }
                catch(Exception e) {

                }
            }

            InsertLog(log);

        }
    }
}