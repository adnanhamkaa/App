using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Model {
    public class SynchronizerLog : ModelBase {
        public string Action { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }
        public string ExtSystemName { get; set; }
        public string Message { get; set; }
    }
}
