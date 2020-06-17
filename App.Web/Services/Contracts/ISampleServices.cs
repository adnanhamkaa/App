using App.DataAccess;
using App.Utilities.Model;
using App.Web.Areas.Samples.Model;
using System.Collections.Generic;

namespace App.Web.Services.Contracts
{
    public interface IMovieServices : ICrudServices<Movie,MovieForm>
    {
        IEnumerable<string> GetCategories();
    }
    public interface ITheatreServices : ICrudServices<Theatre, TheatreForm>
    {

    }
    public interface IShowTimeServices : ICrudServices<ShowTime, ShowTimeForm>
    {

    }
    public interface ITicketServices : IServiceBase
    {
        TicketForm Insert(TicketForm form);
        IEnumerable<string> GetPaymentMethods();
        CouponForm CheckCoupon(string code);
        ShowTimeForm GetShowtimeDetail(string id);
        FileModel GetInvoice(string id);
        TicketForm GetTicketById(string id);
    }
}
