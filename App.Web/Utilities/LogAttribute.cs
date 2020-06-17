using App.DataAccess;
using App.DataAccess.Model;
using Microsoft.Owin;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : ActionFilterAttribute {

        public const string LOGDATA_TEMP_KEY = "logdatakey";
        private string Action = null;
        private ActivityLogModule Module;

        public LogAttribute() { Action = null; }
        public LogAttribute(string action = null) {
            Action = action;
            Module = ActivityLogModule.Default;
        }

        public LogAttribute(string action = null, ActivityLogModule module = ActivityLogModule.Default) {
            Action = action;
            Module = module;
        }
        
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            try {

                string message = null;

                var moduleAttr = filterContext.Controller.GetType().GetCustomAttributes(typeof(ModuleAttribute), true).FirstOrDefault() as ModuleAttribute;

                try {
                    message = filterContext.Controller.TempData[LOGDATA_TEMP_KEY]?.ToString();
                } catch (Exception exc) {

                }

                var url = filterContext.HttpContext.Request.Url.AbsoluteUri;
                var ip = filterContext.HttpContext.Request.UserHostAddress;
                var hostname = filterContext.HttpContext.Request.UserHostName;
                var userName = HttpContext.Current.User.Identity.Name;
                var modul = filterContext.RouteData.Values["area"]?.ToString();

                if (moduleAttr != null) {
                    modul = moduleAttr.Name?.Replace("_", " ");
                }

                //if(Module == ActivityLogModule.Default) {
                //    modul = null;
                //} else {
                //    modul = string.Join(" ",Module.ToString().Split('_'));
                //}



                var context = new ApplicationDbContext();

                var log = new ActivityLog();

                log.Action = Action;
                log.Url = url;
                log.IPAddress = ip;
                log.HostName = hostname;
                log.Modul = modul;
                log.Username = userName;
                log.CreatedDate = DateTime.Now;
                log.Data = message;

                context.ActivityLogs.Add(log);

                context.SaveChanges();

            } catch (Exception exc) {

                
            } finally {

                base.OnActionExecuted(filterContext);
            }

        }
        
        private string GetClientIpAddress(HttpRequestMessage request) {
            if (request.Properties.ContainsKey("MS_HttpContext")) {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            if (request.Properties.ContainsKey("MS_OwinContext")) {
                return IPAddress.Parse(((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress).ToString();
            }
            return String.Empty;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute {
        public string Name { get; set; }
        
    }

    public enum ActivityLogModule {
        Equity_Listing,
        Equity_Suspend_Active,
        Equity_Delisting,
        Equity_Securities_Name,
        Equity_Corporate_Action,
        User_Management_Login,
        User_Management,
        User_Management_Roles,
        Activity_Log,
        Setting_Workday,
        Default
    }
}