using App.DataAccess;
using App.Web.Areas.Samples.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System.Web.Mvc;

namespace App.Web.Areas.Samples.Controllers
{
    [AppAuth]
    [Module(Name = AppActions.Samples_masterdata)]
    public class MovieController : BaseCrudController<IMovieServices, MovieViewModel, Movie, MovieForm>
    {
        public override ActionResult Index() {
            return base.Index();
        }

        public override ActionResult Entry(string id, string opt) {
            return base.Entry(id, opt);
        }

        [HttpPost]
        [Log("Input Movie")]
        public override ActionResult Entry(MovieViewModel vm) {
            var result = base.EntryAction(vm);
            SetActivityLogData(result.ViewModel?.Form?.Name);

            if(ModelState.IsValid)
                return result.Action.Success("Meninput Movie "+ result.ViewModel.Form?.Name);
            else
                return result.Action;
        }

        [HttpPost]
        [Log("Delete Movie")]
        public override ActionResult Delete(string id) {
            var result = base.DeleteAction(id);
            SetActivityLogData(result.ViewModel?.Form?.Name);
            return result.Action.Success("Menghapus Movie "+ result.ViewModel?.Form?.Name);
        }

        [HttpPost]
        public override ActionResult GetList(DttRequestForm form) {
            return base.GetList(form);
        }

        protected override MovieViewModel ViewModelLoader(MovieViewModel vm) {
            vm = base.ViewModelLoader(vm);

            vm.Categories = Service.GetCategories();

            return vm;
        }

    }
}