using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.DataAccess
{
    public class Movie : ModelBase
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int Rating { get; set; }
        [JsonIgnore]
        public ICollection<ShowTime> ShowTimes { get; set; }
    }

    public class Theatre : ModelBase
    {
        public string Name { get; set; }
        public string NumberOfSeat { get; set; }
        [JsonIgnore]
        public ICollection<ShowTime> ShowTimes { get; set; }
    }

    public class ShowTime : ModelBase
    {
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public Movie Movie { get; set; }
        public Theatre Theatre { get; set; }

    }

    public class Ticket : ModelBase
    {
        public string PaymentMethod { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string PromoCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public ShowTime ShowTime { get; set; }
        public string SeatNumber { get; set; }
    }


}
