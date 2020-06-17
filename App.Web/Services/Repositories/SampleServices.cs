using App.DataAccess;
using App.Utilities.Model;
using App.Web.Areas.Samples.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Web.Services.Repositories
{
    public class MovieServices : CrudServiceBase<Movie,MovieForm>, IMovieServices
    {
        public MovieServices(ApplicationDbContext ctx, IMasterDataServices setupSvc, IWordTextReplacementServices wordSvc) 
            : base(ctx, setupSvc, wordSvc) { }


        public IEnumerable<string> GetCategories() {
            yield return "Sci-Fi";
            yield return "Comedy";
            yield return "Horror";
            yield return "Drama";
            yield return "Thriller";
            yield return "Biography";
        }
    }

    public class TheatreServices : CrudServiceBase<Theatre, TheatreForm>, ITheatreServices
    {
        public TheatreServices(ApplicationDbContext ctx, IMasterDataServices setupSvc, IWordTextReplacementServices wordSvc)
            : base(ctx, setupSvc, wordSvc) { }
    }

    public class ShowTimeServices : CrudServiceBase<ShowTime, ShowTimeForm>, IShowTimeServices
    {
        public override string[] Includes => new string[] { "Movie", "Theatre" };

        public ShowTimeServices(ApplicationDbContext ctx, IMasterDataServices setupSvc, IWordTextReplacementServices wordSvc)
            : base(ctx, setupSvc, wordSvc) { }

    }

    public class TicketServices : ServiceBase, ITicketServices
    {
        IWordTextReplacementServices _wordSvc;

        public TicketServices(IWordTextReplacementServices wordSvc) : base() {
            _wordSvc = wordSvc;
        }

        public TicketForm Insert(TicketForm form) {
            var result = context.Set<Ticket>()
                .Include("ShowTime")
                .Where(t => t.Id == form.Id).FirstOrDefault();

            if (result == null) {
                
                form.SetCreated();
                result = new Ticket();
                result = Mapper.Map(form, result);
                result.Init();
                result.ShowTime = context.ShowTimes.FirstOrDefault(t => t.Id == form.ShowTimeId);
                context.Tickets.Add(result);
            } else {
                form.SetUpdated();
                result.ShowTime = context.ShowTimes.FirstOrDefault(t => t.Id == form.ShowTimeId);
                result = Mapper.Map(form, result);
            }

            context.SaveChanges();

            return Mapper.Map<TicketForm>(result);

        }

        public ShowTimeForm GetShowtimeDetail(string id) {
            if (id == null) return null;
            var result = context.ShowTimes.FirstOrDefault(t => t.Id == id);
            if (result != null)
                return Mapper.Map<ShowTimeForm>(result);
            else
                return null;
        }

        public CouponForm CheckCoupon(string code) {

            //get data from db
            return GetVoucherFromDB(code);
            
        }

        public CouponForm GetVoucherFromDB(string code) {
            var potongan = (decimal)Math.Round((double)Randomer.GlobalSelf.Next(10000, 15000) / 100, 0) * 100;

            return new CouponForm() {
                Coupon = code,
                Potongan = potongan
            };
        }

        public IEnumerable<string> GetPaymentMethods() {
            yield return "On The Spot";
            yield return "Virtual Account";
            yield return "Debit/Credit";
            yield return "My Wallet";
        }

        public TicketForm GetTicketById(string id) {

            var ticket = context.Tickets
                .Include("ShowTime")
                .Include("ShowTime.Movie")
                .Include("ShowTime.Theatre")
                .FirstOrDefault(t => t.Id == id);

            return Mapper.Map<TicketForm>(ticket);

        }

        public FileModel GetInvoice(string id) {

            var ticket = context.Tickets
                .Include("ShowTime")
                .Include("ShowTime.Movie")
                .Include("ShowTime.Theatre")
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null) return null;

            var showtimeForm = Mapper.Map<ShowTimeForm>(ticket.ShowTime);

            var replaceItem = new List<WordReplacement>();

            replaceItem.Add(new WordReplacement("OrderNum", ticket.CreatedDate.ToString("yyyyMMddHHmm") ?? ""));
            replaceItem.Add(new WordReplacement("TicketDesc", showtimeForm.ToString() ?? ""));
            replaceItem.Add(new WordReplacement("InvDate", ticket.CreatedDate.ToStringIndonesia("dd MMM yyyy") ?? ""));
            replaceItem.Add(new WordReplacement("SeatNum", ticket.SeatNumber ?? ""));
            replaceItem.Add(new WordReplacement("TicketPrice", ticket.OriginalPrice?.ToFormatedString(addRupiah:true) ?? ""));
            replaceItem.Add(new WordReplacement("TotalPrice", ticket.OriginalPrice?.ToFormatedString(addRupiah: true) ?? ""));
            replaceItem.Add(new WordReplacement("InvDue", ticket.CreatedDate.AddMinutes(90).ToStringIndonesia("dd MMM yyyy HH:mm") ?? ""));
            replaceItem.Add(new WordReplacement("FDiscount", ticket.Discount?.ToFormatedString(addRupiah: true) ?? ""));
            replaceItem.Add(new WordReplacement("FPrice", ticket.Price?.ToFormatedString() ?? ""));
            replaceItem.Add(new WordReplacement("PaymentMethod", ticket.PaymentMethod ?? "")); 
                replaceItem.Add(new WordReplacement("PromoCode", ticket.PromoCode ?? "")); 

            return new FileModel() {
                FileName = $"Invoice {ticket.CreatedDate.ToString("yyyyMMddHHmm")}.docx",
                Mime = FileModel.MIME_DOCX,
                Bytes = _wordSvc.GetWordReplacedText(Templates.TICKET,replaceItem)
            };

        }

    }
}