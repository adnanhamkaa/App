using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Model {
    public class Workflow : ModelBase {
        public string Type { get; set; }
        public string DataId { get; set; }
        public ICollection<Flow> Flows { get; set; }
    }

    public class Flow : ModelBase {
        public int Order { get; set; }
        public string Keterangan { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Roles { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public DateTime? DueDate { get; set; }
        
        public string EmailGuid { get; set; }
        public int? EmailSequence { get; set; }
        public bool? MailSent { get; set; }
        public string RoleRecipient { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public bool? IsSendEmail { get; set; }
        public string Type { get; set; }
        public string DisplayGrouping { get; set; }
    }
}
