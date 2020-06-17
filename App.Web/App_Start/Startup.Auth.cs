using System;
using App.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using App.DataAccess;
using Microsoft.AspNet.Identity.EntityFramework;
using App.DataAccess.Identity;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using App.Web.Utilities;
using Npgsql;
using System.Configuration;

namespace App.Web {
    public partial class Startup {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app) {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext(App.DataAccess.ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, App.DataAccess.Identity.ApplicationUser>(
                            TimeSpan.FromMinutes(WebAppSettings.SessionTimeout),
                            (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                ExpireTimeSpan = TimeSpan.FromMinutes(WebAppSettings.SessionTimeout),
                SlidingExpiration = true
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
            
        }
        
        public void CreateRolesandUsers() {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();

                var roleManager = new RoleManager<ApplicationRole, string>(new RoleStore<ApplicationRole, string, IdentityUserRole>(context));
                var UserManager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context));


                var actions = new List<string>();
                var spopaction = new AppActions();
                foreach (var prop in typeof(AppActions).GetFields())
                {
                    if (prop.IsPublic)
                    {
                        var value = prop.GetValue(spopaction) as string;
                        actions.Add(value);
                    }
                }

                var dbActions = context.ActionAuthorization.Where(t => actions.Any(a => a == t.ActionName)).ToList();

                actions.Where(t => !dbActions.Any(a => a.ActionName == t)).ToList().ForEach(t =>
                {

                    var action = new ActionAuthorization();
                    action.ActionName = t;
                    action.Init();

                    context.ActionAuthorization.Add(action);

                });

                dbActions.Where(t => !actions.Any(a => a == t.ActionName)).ToList().ForEach(t => {
                    t.Delete("");
                });



                var notifrole = new List<string>();
                foreach (var enums in CommonHelper.GetEnumValues<AppReminderTypes>()) {
                    notifrole.Add(enums.ToString());
                }

                var dbNotifRole = context.Roles.Where(t => notifrole.Any(a => a == t.Name)).Select(t => t.Name).ToList();

                dbNotifRole.Where(t => !dbNotifRole.Any(a => a == t)).ToList().ForEach(t => {
                    var role = new ApplicationRole();
                    role.Name = t;
                    role.Id = Guid.NewGuid().ToString();

                    var roleresult = roleManager.Create(role);
                    
                });

                context.SaveChanges();

                if (!roleManager.RoleExists("Admin"))
                {

                    var role = new ApplicationRole();
                    role.Name = "Admin";
                    role.Id = "22950b4d-6ffc-4d56-bbeb-b9d8a9209454";

                    var roleresult = roleManager.Create(role);

                    var user = new ApplicationUser();
                    user.FullName = "admin";
                    user.Email = ConfigurationManager.AppSettings["EmailAdmin"]?.ToString();
                    user.UserName = user.Email;
                    user.IsActive = true;

                    string userPWD = "Aspapp@789";

                    var chkUser = UserManager.Create(user, userPWD);

                    if (chkUser.Succeeded)
                    {
                        var result1 = UserManager.AddToRole(user.Id, "Admin");
                    }

                    var registAction = context.ActionAuthorization.Where(t => t.ActionName == AppActions.usermanagement_registrasi).FirstOrDefault();

                    var roleObj = context.Roles.Include("Actions").Where(t => t.Name == "Admin").FirstOrDefault();

                    if (registAction != null)
                    {
                        roleObj.Actions.Add(registAction);

                    }

                    var roleAction = context.ActionAuthorization.Where(t => t.ActionName == AppActions.User_Management_Roles).FirstOrDefault();

                    if (roleAction != null) {
                        
                        roleObj.Actions.Add(roleAction);

                    }


                    context.SaveChanges();
                }
            } catch (Exception startUpDbException)
            {
                // any db exception related when db not initialized yet
            }

            //// creating Creating Manager role    
            //if (!roleManager.RoleExists("Manager")) {
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Manager";
            //    roleManager.Create(role);
            //}

            //// creating Creating Employee role    
            //if (!roleManager.RoleExists("Employee")) {
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Employee";
            //    roleManager.Create(role);
            //}
        }
    }
}