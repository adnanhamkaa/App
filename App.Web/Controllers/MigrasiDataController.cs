using App.Web.Services.Contracts;
using App.Web.Utilities;

namespace App.Web.Controllers
{
    public class MigrasiDataController : BaseController
    {
        private IMigrasiDataServices _service;

        public MigrasiDataController(IMigrasiDataServices service) {
            _service = service;
        }

    }
}