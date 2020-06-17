using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities.Settings {
    public class ChecklistFieldSetting : FormFieldSetting {
        /// <inheritdoc />
        public override string TemplateId => "checklist";

        public IEnumerable<object> Options { get; set; }
        public Func<object, string> OptionLabelGetter { get; set; }
        public Func<object, string> OptionValueGetter { get; set; }
        
    }
}