using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Model {
    public class ActivityLog : ModelBase {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Int64 Id { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Modul { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        //public DateTime CreatedDate { get; set; }
    }
}
