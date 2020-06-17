using App.DataAccess;
using App.Utilities;
using App.Utilities.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using Elmah;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Web.Utilities
{
    public abstract class BaseController : System.Web.Mvc.Controller {

        protected string LOG_DATA_KEY = LogAttribute.LOGDATA_TEMP_KEY;

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior) {
            
            return new JsonDotNetResult {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                RecursionLimit = 10
            };
        }

        

        protected IServiceBase _serviceBase;

        public BaseController() {
        }
        public BaseController(IServiceBase baseservice) { _serviceBase = baseservice; }

        public ActionResult File(FileModel model) {

            return File(model?.Bytes, model?.Mime, System.IO.Path.GetFileName(model?.FileName));
            
        }

        public FileContentResult InlineFile(FileModel model) {
            var cd = new System.Net.Mime.ContentDisposition {
                // for example foo.bak
                FileName = model.FileName,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
                
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(model.Bytes,model.Mime);
        }
        
        [HttpPost]
        public virtual ActionResult RestoreDelete(string Id, string menu) {
            _serviceBase.RestoreDelete(Id);
            return RedirectToAction("Index").Success($"Melakukan Restore {menu}");
        }

        protected void SetActivityLogData(string data) {
            TempData[LOG_DATA_KEY] = data;
        }
        
        public void ErrorLog(Exception e) {
            ErrorSignal.FromCurrentContext().Raise(e);
        }

        protected void SetBreadCrumbs(params BreadCrumbItem[] crumbs) {
            ViewBag.CustomBreadCrumbs = crumbs.ToList();
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Main Service</typeparam>
    public class BaseController<T> : BaseController where T : IServiceBase
    {
        
        public T Service { get; set; }

        [Inject]
        public void Arm(T svc) {
            if(Service == null)
                this.Service = svc;
        }

        public BaseController() { }

        public BaseController(T service) : base(service) {
            Service = service;
        }
    }

    public class BaseController<TService, TViewModel> : BaseController where TService : IServiceBase where TViewModel : ViewModelBase, new()
    {

        public TService Service { get; set; }

        [Inject]
        public void Arm(TService svc) {
            if (Service == null)
                this.Service = svc;
        }

        public BaseController() { }

        public BaseController(TService service) : base(service) {
            Service = service;
        }

        protected virtual TViewModel ViewModelLoader(TViewModel vm = null) {
            if (vm == null) vm = new TViewModel();
            return vm;
        }
    }


    public class BaseCrudController<TSerivce, TViewModel, TModel, TForm> : BaseController<TSerivce,TViewModel>
        where TSerivce : ICrudServices<TModel, TForm>
        where TViewModel : ViewModelBase<TForm>, new()
        where TModel : ModelBase, new()
        where TForm : FormModelBase, new()
    {

        
        public TSerivce Service { get; set; }

        [Inject]
        public void Inject(TSerivce svc) {
            if (Service == null)
                this.Service = svc;
        }

        // GET: MasterData/Board
        protected virtual CrudActionResult<TViewModel, TForm> IndexAction() {
            return new CrudActionResult<TViewModel, TForm>() { Action = View() };
        }

        protected virtual CrudActionResult<TViewModel, TForm> GetListAction(DttRequestForm form) {
            return new CrudActionResult<TViewModel, TForm>() { Action = Json(Service.GetDynamicList(form)), ViewModel = null };
        }

        protected virtual CrudActionResult<TViewModel, TForm> EntryAction(string id, string opt) {
            var vm = ViewModelLoader(new TViewModel());
            vm.Form = Service.GetById(id);

            if (opt == "clone") {
                vm.Form.Id = null;
            }

            return new CrudActionResult<TViewModel, TForm>() { Action = View(vm), ViewModel = vm };
        }

        protected virtual CrudActionResult<TViewModel, TForm> EntryAction(TViewModel vm) {

            if (!vm.Form.IsDraft && !ModelState.IsValid) {
                return new CrudActionResult<TViewModel, TForm>() { Action = View(vm), ViewModel = ViewModelLoader(vm) };
            }

            if (Service.CheckIsSameDataExists(vm.Form)) {
                ModelState.AddModelError("", "Duplicate Data"); ;
                return new CrudActionResult<TViewModel, TForm>() { Action = View(vm), ViewModel = ViewModelLoader(vm) };
                 
            }

            vm.Form = Service.Insert(vm.Form);
            return new CrudActionResult<TViewModel, TForm>() { Action = RedirectToAction("Index"), ViewModel = ViewModelLoader(vm) };
        }

        protected virtual CrudActionResult<TViewModel,TForm> DeleteAction(string id) {
            var result = Service.Delete(id);
            return new CrudActionResult<TViewModel, TForm>(){
                Action = RedirectToAction("Index"),
                ViewModel = new TViewModel() { Form = result }
            };
        }

        [HttpGet]
        public virtual ActionResult Index() {
            return IndexAction().Action;
        }
        [HttpPost]
        public virtual ActionResult GetList(DttRequestForm form) {
            return GetListAction(form).Action;
        }
        [HttpGet]
        public virtual ActionResult Entry(string id, string opt) {
            return EntryAction(id, opt).Action;
        }
        [HttpPost]
        public virtual ActionResult Entry(TViewModel vm) {
            return EntryAction(vm).Action;
        }
        [HttpPost]
        public virtual ActionResult Delete(string id) {
            return DeleteAction(id).Action;
        }

        public BaseCrudController() : base() { }

        //public BaseCrudController(TSerivce service) {
        //    Service = service;
        //}

    }

    public class BreadCrumbItem
    {
        public string Url { get; set; }
        public string Display { get; set; }
        public BreadCrumbItem(string url, string display) {
            this.Url = url;
            this.Display = display;
        }
    }

    public class CrudActionResult<TVm, Tf>
        where TVm : ViewModelBase<Tf>
        where Tf : FormModelBase

    {
        public ActionResult Action { get; set; }
        public TVm ViewModel { get; set; }
    }
}

