using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class WorkflowForm : FormModelBase {
        
        public const string WF_STATUS_CREATED = "created";
        public const string WF_STATUS_DONE = "done";

        public string Type { get; set; }
        public string DataId { get; set; }
        public IEnumerable<FlowForm> Flows { get; set; }
    }

    public class FlowForm : FormModelBase
    {
        
        public int Order { get; set; }
        public string Keterangan { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public DateTime? DueDate { get; set; }
        public string IcsGuid { get; set; }

        public string EmailGuid { get; set; }
        public int? EmailSequence { get; set; }
        public bool? MailSent { get; set; }
        public string RoleRecipient { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public string DisplayGrouping { get; set; }

        public bool? IsSendEmail { get; set; }

        public FlowForm() : base() {
            Status = WorkflowForm.WF_STATUS_CREATED;
            Id = Guid.NewGuid().ToString();
            this.SetCreated();
            IsSendEmail = true;
        }

    }

    public class ToDoForm : FormModelBase
    {

        public int Order { get; set; }
        public string Keterangan { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public DateTime? DueDate { get; set; }
        public string Type { get; set; }
        public string DisplayGrouping { get; set; }


        public ToDoForm() : base() {
            Status = WorkflowForm.WF_STATUS_CREATED;
            Id = Guid.NewGuid().ToString();
            this.SetCreated();
        }

    }
}