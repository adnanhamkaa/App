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
    [Module(Name = AppModule.Samples_Ticket)]
    public class TicketController : BaseController<ITicketServices, TicketViewModel>
    {
        IMasterDataServices _masterDataSvc;
        public TicketController(IMasterDataServices masterDataSvc) : base() {
            _masterDataSvc = masterDataSvc;
        }

        public ActionResult Index() {
            return View(ViewModelLoader());
        }

        [HttpPost]
        [Log("Order Ticket")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TicketViewModel vm) {

            vm = ViewModelLoader(vm);

            if (!ModelState.IsValid) {
                return View(vm);
            }

            if (Service.IsSameDataExists<Ticket,TicketForm>(vm.Form)) {
                ModelState.AddDuplicateErrorState();
                return View(vm);
            }

            var result = Service.Insert(vm.Form);
            SetActivityLogData(result?.ShowTimeId);
            return RedirectToAction("Invoice",new { id = result.Id }).Success("Melakukan order");

        }

        [HttpGet]
        public ActionResult Invoice(string id) {
            SetBreadCrumbs(new BreadCrumbItem("/Samples/Ticket", "Order Ticket"),
                new BreadCrumbItem("/Samples/Ticket/Invoice/" + id, "Invoice"));
            return View(new TicketViewModel() { Form = Service.GetTicketById(id) });

        }


        //Ajax Sample
        [HttpPost]
        public ActionResult CheckCoupon(string id) {

            return Json(Service.CheckCoupon(id));
        }

        [HttpPost]
        public ActionResult ShowtimeDetail(string id) {

            return Json(Service.GetShowtimeDetail(id));
        }

        protected override TicketViewModel ViewModelLoader(TicketViewModel vm = null) {
            vm = base.ViewModelLoader(vm);

            vm.ShowTimes = _masterDataSvc.GetAll<ShowTime, ShowTimeForm>(false, "Movie", "Theatre");
            vm.PaymentMethods = Service.GetPaymentMethods();

            return vm;
        }

        public ActionResult GetInvoice(string id) {

            if (id == null) return Redirect(Request.UrlReferrer.ToString()).Error("Invoice Tidak Ditemukan");

            var result = Service.GetInvoice(id);

            if (result?.Bytes == null) return Redirect(Request.UrlReferrer?.ToString()??"/").Error("Invoice Tidak Ditemukan");

            return File(Service.GetInvoice(id));
        }
    }
}