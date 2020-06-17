using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using App.DataAccess;
using App.DataAccess.Identity;
using App.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace App.Web {
    public partial class Startup {
        public void ConfigureHangfire(IAppBuilder app) {
            //GlobalConfiguration.Configuration.UsePostgreSqlStorage("DefaultConnection");
            //GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });
            //JobStorage.Current = new PostgreSqlStorage("DefaultConnection");
            //app.UseHangfireServer(new BackgroundJobServerOptions(),
            //    new PostgreSqlStorage("DefaultConnection"));
            app.UseHangfireDashboard($"/hangfire{ConfigurationManager.AppSettings["App:HangfireSecret"].ToString()}", new DashboardOptions() {
                DisplayStorageConnectionString = false,
                Authorization = new[] { new MyAuthorizationFilter() }
            });

        }
    }

    public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient {
        public void Preload(string[] parameters) {
            HangfireBootstrapper.Instance.Start();
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter {
        public bool Authorize(DashboardContext context) {
            // In case you need an OWIN context, use the next line, `OwinContext` class
            // is the part of the `Microsoft.Owin` package.

            var owinContext = new OwinContext(context.GetOwinEnvironment());

            var user = owinContext.GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            
            if (user == null) {
                return false;
            }

            List<ApplicationRole> roles = null;

            if (HttpContext.Current.Cache["roles"] != null && false) {
                roles = (List<ApplicationRole>)HttpContext.Current.Cache["roles"];
            } else {
                var dbcontext = new ApplicationDbContext();
                roles = dbcontext.Roles.Include("Actions").ToList();
                HttpContext.Current.Cache["roles"] = roles;
            }

            var authorizedActions = roles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList().SelectMany(t => t.Actions).ToList();

            return authorizedActions.Any(t => t.ActionName == AppActions.Background_Job);
            
        }
    }

    public class HangfireBootstrapper : IRegisteredObject {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper() {
        }

        public void Start() {
            lock (_lockObject) {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                GlobalConfiguration.Configuration.UsePostgreSqlStorage("DefaultConnection");
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });
                JobStorage.Current = new PostgreSqlStorage("DefaultConnection");
                
                _backgroundJobServer = new BackgroundJobServer();
            }
        }

        public void Stop() {
            lock (_lockObject) {
                if (_backgroundJobServer != null) {
                    _backgroundJobServer.Dispose();
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate) {
            Stop();
        }
    }
}