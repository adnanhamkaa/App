using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public abstract class FormFieldSetting {
        public abstract string TemplateId { get; }
        public string InputName { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string HelpText { get; set; }
        public string CssClass { get; set; }
        public string HtmlId { get; set; }
        public bool Disabled { get; set; }
        public string ReferenceFrom { get; set; }
        public bool Changed { get; set; }
        public bool ChangedWatch { get; set; }
        public bool SeparatorNumber { get; set; }
        public string CssInput { get; set; }

        public FormFieldSetting() {
            ChangedWatch = true;
            SeparatorNumber = false;
        }
    }
}