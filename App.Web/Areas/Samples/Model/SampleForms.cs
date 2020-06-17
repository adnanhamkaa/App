using App.DataAccess;
using App.Web.Models;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Areas.Samples.Model
{
    public class MovieForm : FormModelBase
    {
        [AppRequired]
        public string Name { get; set; }
        public string Category { get; set; }
        [DttField(renderFunction:"languageRender")]
        public string Language { get; set; }
        [AppRequired]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }
        public int Rating { get; set; }
        [DttField(hidden: true)]
        public string HiddenProp { get; set; }
    }

    public class TheatreForm : FormModelBase
    {
        [AppRequired]
        public string Name { get; set; }
        [Display(Name = "Number of Seat")]
        public string NumberOfSeat { get; set; }
    }

    public class ShowTimeForm : FormModelBase {
        [AppRequired]
        public DateTime? Date { get; set; }
        [AppRequired]
        public decimal? Price { get; set; }

        [DttField(hidden: true)]
        [AppRequired]
        [Display(Name = "Movie")]
        public string MovieId { get; set; }

        [DttField(hidden: true)]
        [AppRequired]
        [Display(Name = "Theatre")]
        public string TheatreId { get; set; }

        [DttField(renderChildren: new string[]{ "Name:Movie" })]
        [IdField("MovieId")]
        public Movie Movie { get; set; }

        [DttField(renderChildren: new string[] { "Name:Theatre" })]
        [IdField("TheatreId")]
        public Theatre Theatre { get; set; }

        public override string ToString() {
            return $"{this.Movie?.Name} - {Theatre?.Name} - {Date?.ToStringIndonesia("dddd d MMM HH:mm")}";
        }

        public static string GetDescFromModel(ShowTime model) {
            return $"{model?.Movie?.Name} - {model?.Theatre?.Name} - {model?.Date?.ToStringIndonesia("dddd d MMM HH:mm")}";
        }
    }

    public class TicketForm : FormModelBase
    {
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [AppRequired]
        [Display(Name = "Original Price")]
        public decimal? OriginalPrice { get; set; }

        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }

        [AppRequired]
        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }

        [AppRequired]
        [Display(Name = "Show Time")]
        public string ShowTimeId { get; set; }

        [IdField("ShowTimeId")]
        public ShowTime ShowTime { get; set; }

        [Display(Name = "Seat Number")]
        public string SeatNumber { get; set; }
    }

    public class CouponForm : FormModelBase
    {
        public string Coupon { get; set; }
        public decimal? Potongan { get; set; }
        
    }
}