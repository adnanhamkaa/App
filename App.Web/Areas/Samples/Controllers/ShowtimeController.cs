using App.DataAccess;
using App.Web.Areas.Samples.Model;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Areas.Samples.Controllers
{
    [AppAuth]
    [Module(Name = AppModule.Samples_MasterData)]
    public class ShowtimeController : BaseCrudController<IShowTimeServices, ShowTimeViewModel, ShowTime, ShowTimeForm>
    {
        private IMasterDataServices _setupSvc;

        public ShowtimeController(IMasterDataServices setupSvc) : base() {
            _setupSvc = setupSvc;
        }

        protected override ShowTimeViewModel ViewModelLoader(ShowTimeViewModel vm) {

            vm.Movies = _setupSvc.GetAll<Movie>();
            vm.Theatres = _setupSvc.GetAll<Theatre>();

            return vm;

        }

    }
}