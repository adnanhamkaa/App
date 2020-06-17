using App.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace App.Web {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
            CreateRolesandUsers();
            ConfigureHangfire(app);
            StartCronJobs();
            InitData();
        }
    }
}