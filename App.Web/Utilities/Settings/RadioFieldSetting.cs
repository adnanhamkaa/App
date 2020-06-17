using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities.Settings {
    public class RadioFieldSetting : FormFieldSetting {
        /// <inheritdoc />
        public override string TemplateId => "radio";        
        public IEnumerable<object> Options { get; set; }
        public Func<object, string> OptionLabelGetter { get; set; }
        public Func<object, string> OptionValueGetter { get; set; }
    }
}