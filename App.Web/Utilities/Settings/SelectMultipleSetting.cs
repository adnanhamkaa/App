using System;
using System.Collections.Generic;

namespace App.Web.Utilities.Settings {
    public class SelectMultipleSetting : FormFieldSetting {
        public SelectMultipleSetting() : base() {
            EmptyOptionLabel = "-kosong-";
        }
        /// <inheritdoc />
        public override string TemplateId => "selectmultiple";
        public IEnumerable<object> Options { get; set; }
        public Func<object, string> OptionLabelGetter { get; set; }
        public Func<object, string> OptionValueGetter { get; set; }
        public bool AddEmptyOption { get; set; }
        public string EmptyOptionLabel { get; set; }
        public bool Select2 { get; set; }
        public string SelectedText { get; set; }
        public string Select2Url { get; set; }
        public bool Select2Ajax { get; set; }
    }
}