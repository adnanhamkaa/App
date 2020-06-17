using App.DataAccess.Model;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    [DttTableOption(useActionColumn:false,useStatusColumn:false)]
    public class ActivityLogModel : FormModelBase {
        //public Int64 Id { get; set; }
        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }

        [Display(Name = "Host Name")]
        public string HostName { get; set; }

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "Modul")]
        public string Modul { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

        [Display(Name = "Data")]
        public string Data { get; set; }
        
        //[Display(Name = "Data")]
        //public string CreatedDate { get; set; }

        //public DateTime CreatedDate { get; set; }
    }
  
}