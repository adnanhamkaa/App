using System;
using System.Web.Mvc;

namespace App.Web.Utilities.Settings {
    public class DateTimeFieldSetting : FormFieldSetting {
        public DateTimeFieldSetting() : base() {
            Style = DateTimeFieldStyle.Complete;
            //MinDate = DateTime.Today;     //dont set MinDate in base, cause not all DateTimeFieldSetting using that rule.
        }

        /// <inheritdoc />
        public override string TemplateId => "datetime";

        public DateTimeFieldStyle Style { get; set; }

        public bool UseVerticalFormFieldFormat { get; set; }
        public DateTime MinDate { get; set; }
        public bool Inline { get; set; }
        public bool? UseWorkday { get; set; }
        public string InlineContainer { get; set; }
        public MvcHtmlString LeftAddon { get; set; }
        public WorkdayModule? WorkdayModule { get; set; }
        public bool? ResetButton { get; set; }
        public bool? MultipleDate { get; set; }
        public string InitCallback { get; set; }
    }

    public enum DateTimeFieldStyle {
        Complete = 0,
        DateOnly = 1,
        TimeOnly = 2,
        MonthOnly = 3,
        YearOnly = 4
    }
     public enum WorkdayModule {
        Equity,
        Derivatif,
        Indeks,
        Announcement,
        Obligasi
    }
}