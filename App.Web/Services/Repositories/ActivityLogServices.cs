using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DataAccess;
using AutoMapper;
using App.DataAccess.Model;

namespace App.Web.Services.Repositories {
    public class ActivityLogServices : ServiceBase, IActivityLogServices {


        public ActivityLogServices(ApplicationDbContext context) {
            this.context = context;
        }

        public DttResponseForm<ActivityLogModel> GetList(DttRequestWithDate form) {
            return ProcessDatatableResult<ActivityLogModel, ActivityLog>(context.ActivityLogs.AsQueryable(), form);
        }

        //public void InsertLog(ActivityLogModel activityLogModel) {
        //    using (ApplicationDbContext context = new ApplicationDbContext()) {
        //        context.ActivityLogs.Add(activityLogModel.GetActivityLog());
        //        context.SaveChanges();
        //    }
        //}

        //public void InsertLog(string modul, string action, string data) {
        //    using(ApplicationDbContext context = new ApplicationDbContext()) {
        //        context.ActivityLogs.Add(new DataAccess.Model.ActivityLog() {
        //            IPAddress = HttpContext.Current.Request.UserHostAddress,
        //            HostName = HttpContext.Current.Request.UserHostAddress,
        //            Username = HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : null,
        //            Modul = modul,
        //            Action = action,
        //            Data = data,
        //            CreatedDate = DateTime.Now
        //        });

        //        context.SaveChanges();
        //    }
        //}
    }
}