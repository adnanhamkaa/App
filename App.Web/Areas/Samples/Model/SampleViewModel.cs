using App.DataAccess;
using App.Web.Models;
using System.Collections.Generic;

namespace App.Web.Areas.Samples.Model
{
    public class MovieViewModel : ViewModelBase<MovieForm>
    {
        public IEnumerable<string> Categories { get; set; }
    }

    public class TheatreViewModel : ViewModelBase<TheatreForm>
    {

    }

    public class ShowTimeViewModel : ViewModelBase<ShowTimeForm>
    {
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<Theatre> Theatres { get; set; }
    }

    public class TicketViewModel : ViewModelBase<TicketForm>
    {
        public string TicketDesc { get; set; }
        public IEnumerable<ShowTimeForm> ShowTimes { get; set; }
        public IEnumerable<string> PaymentMethods { get; set; }
    }

}