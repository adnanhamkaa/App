using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Ninject.Modules;
using Ninject.Web.Common;

namespace App.Web.Services {
    public class IdentityNinjectModule : NinjectModule {
        /// <inheritdoc />
        public override void Load() {
            Kernel.Bind<ApplicationUserManager>().ToMethod(ctx => GetUserManagerFromOwinContext())
                .InRequestScope();
            Kernel.Bind<ApplicationSignInManager>().ToMethod(ctx => GetSignInManagerFromOwinContext())
                .InRequestScope();
            Kernel.Bind<ApplicationRoleManager>().ToMethod(ctx => GetRoleManagerFromOwinContext())
                .InRequestScope();
        }

        public static ApplicationUserManager GetUserManagerFromOwinContext() {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public static ApplicationSignInManager GetSignInManagerFromOwinContext() {
            return HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
        }

        public static ApplicationRoleManager GetRoleManagerFromOwinContext() {
            return HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
        }
    }
}