using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class DttRequestWithDate : DttRequestForm {

        private DateTime? _startDate;
        private DateTime? _endDate;

        public DateTime? StartDate {
            get { return _startDate?.Date; }
            set { _startDate = value;  }
        }

        public DateTime? EndDate {
            get { return _endDate?.Date.AddHours(23).AddMinutes(59).AddSeconds(59); }
            set { _endDate = value; }
        }
    }
}