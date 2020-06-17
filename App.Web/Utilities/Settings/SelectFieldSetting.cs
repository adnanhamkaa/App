using App.Web.Models;
using System;
using System.Collections.Generic;

namespace App.Web.Utilities.Settings {
    public class SelectFieldSetting : FormFieldSetting {
        public SelectFieldSetting() : base() {
            EmptyOptionLabel = "-kosong-";
        }
        /// <inheritdoc />
        public override string TemplateId => "select";
        public IEnumerable<object> Options { get; set; }
        public Func<object, string> OptionLabelGetter { get; set; }
        public Func<object, string> OptionValueGetter { get; set; }
        public Func<object, string> Attributes { get; set; }
        public bool AddEmptyOption { get; set; }
        public string EmptyOptionLabel { get; set; }
        public bool Select2 { get; set; }
        public string SelectedText { get; set; }
        public string Select2Url { get; set; }
        public bool Select2Ajax { get; set; }
        public string CascadeFrom { get; set; }
        public Func<object, string> CascadeParentIdGetter { get; set; }
    }
}