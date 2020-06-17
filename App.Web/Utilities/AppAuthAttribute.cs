using App.DataAccess;
using App.DataAccess.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace App.Web.Utilities {
    public class AppAuthAttribute: AuthorizeAttribute {
        private bool isHidden = false;
        public bool BypassAction { get; set; }
        public string Action { get; set; }
            
        public AppAuthAttribute() {
            BypassAction = true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext) {
            //base.OnAuthorization(filterContext);
            //if (filterContext.HttpContext.Request.Cookies["ASP.NET_SessionId"] == null &&
            //    (filterContext.HttpContext.Request.Cookies["ASP.NET_SessionId"] != null && filterContext.HttpContext.Request.Cookies["ASP.NET_SessionId"].Expires <= DateTime.Now))
            //{
            //    HandleUnauthorizedRequest(filterContext);
            //    return;
            //}

            var user = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            List<ApplicationRole> roles = null;

            var moduleAttr = filterContext.Controller.GetType().GetCustomAttributes(typeof(ModuleAttribute), true).FirstOrDefault() as ModuleAttribute;

            if (user == null) {
                HandleUnauthorizedRequest(filterContext);
                return;
            }

            if(filterContext.HttpContext.Cache["roles"] != null && false) {
                roles = (List<ApplicationRole>)filterContext.HttpContext.Cache["roles"];
            } else {
                var context = new ApplicationDbContext();
                roles = context.Roles.Include("Actions").ToList();
                filterContext.HttpContext.Cache["roles"] = roles;
            }

            if (!BypassAction) {
                var authorizedActions = roles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList().SelectMany(t => t.Actions).ToList();
                if (moduleAttr != null) {
                    if (!authorizedActions.Any(t => t.ActionName == moduleAttr.Name)) {
                        isHidden = true;
                        HandleUnauthorizedRequest(filterContext);
                        return;
                    }
                } else if (!authorizedActions.Any(t => t.ActionName == this.Action)) {
                    isHidden = true;
                    HandleUnauthorizedRequest(filterContext);
                    return;
                }
            }

            base.OnAuthorization(filterContext);

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            //base.HandleUnauthorizedRequest(filterContext);
            var routeData = filterContext.RouteData;
            var controller = "ErrorPage";
            var action = "403";

            //filterContext.HttpContext.Response.Cookies.Clear();
            //filterContext.HttpContext.Request.GetOwinContext().Authentication.SignOut();
            //filterContext.HttpContext.Session.Clear();

            //HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
            //cookie.Expires = DateTime.Now.AddDays(-1);
            //filterContext.HttpContext.Response.Cookies.Add(cookie);


            //HttpCookie cookie2 = new HttpCookie("__RequestVerificationToken", "");
            //cookie2.Expires = DateTime.Now.AddDays(-1);
            //filterContext.HttpContext.Response.Cookies.Add(cookie2);

            string redirect = null;

            if (routeData.Values != null && !isHidden)
            {
                controller = "Account";
                action = "Login";
                if (routeData.DataTokens != null && routeData.DataTokens.ContainsKey("area"))
                {
                    string area = routeData.DataTokens["area"].ToString();

                    redirect += $"/{area}";
                }

                string redirectController = null;
                string redirectAction = null;

                foreach (var data in routeData.Values)
                {
                    if (data.Key.Equals("controller"))
                    {
                        redirectController = data.Value.ToString();
                    }

                    if (data.Key.Equals("action"))
                    {
                        redirectAction = data.Value.ToString();
                    }
                }

                redirect += $"/{redirectController}/{redirectAction}";
            }

            filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", action },
                                       { "controller", controller },
                                       { "area", null },
                                       { "returnUrl",  redirect}
                                   });
        }

    }

    public class XssFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            try {

                if(filterContext.Result is ViewResult) {
                    var model = (filterContext.Result as ViewResult).Model;
                    ProcessXssString(model,1);
                }

            } catch (Exception exc) {

            } finally {
                base.OnActionExecuted(filterContext);
            }
        }

        private void ProcessXssString(object model, int depth) {
            var strings = model?.GetType().GetProperties().Where(t => t.PropertyType == typeof(string)).ToList();
            var objs = model?.GetType().GetProperties().Where(t => t.PropertyType.IsClass).ToList();
            var enumerables = model?.GetType().GetProperties().Where(t => t.PropertyType == typeof(IEnumerable<object>)).ToList();

            foreach (var item in strings) {
                var val = item?.GetValue(model)?.ToString();
                if (val != null)
                    item?.SetValue(model, System.Web.HttpUtility.HtmlEncode(val));
            }

            if (depth <= 5) {
                foreach (var item in objs) {
                    var val = item?.GetValue(model);
                    if (val == null) continue;
                    ProcessXssString(val,depth+1);
                }

                foreach (var item in enumerables) {
                    var val = (IEnumerable<object>)item?.GetValue(model);
                    if (val == null) continue;
                    processEnumerable(val, depth + 1);
                }
            }
        }

        private void processEnumerable(IEnumerable<object> model, int depth) {
            if (depth <= 5) {
                var list = model.ToList();
                int i = -1;
                foreach (var subitem in model) {
                    i++;
                    if (subitem == null) continue;
                    if (subitem is string) {
                        list[i] = subitem.ToString();
                    } else if (subitem.GetType().IsClass) {
                        ProcessXssString(subitem, depth + 1);
                    } else if (subitem.GetType() == typeof(IEnumerable<object>)) {
                        processEnumerable((IEnumerable<object>)subitem, depth + 1);
                    }

                }

                model = list;
            }
        }

    }


}