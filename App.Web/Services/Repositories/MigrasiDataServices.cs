using App.DataAccess;
using App.Web.Services.Contracts;

namespace App.Web.Services.Repositories
{
    public class MigrasiDataServices : ServiceBase, IMigrasiDataServices {

        public MigrasiDataServices(ApplicationDbContext context) {
            this.context = context;
        }

    }
}