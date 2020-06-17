using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models.Email {
    public class ICSFileModel {
        public string Location { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string IcsGuid { get; set; }
        public string Sequence { get; set; }
        public string Method { get; set; }
    }
    
}