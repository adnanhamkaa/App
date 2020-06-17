using System.Web.Mvc;

namespace App.Web.Utilities.Settings {
    public class FloatFieldSetting : FormFieldSetting {
        /// <inheritdoc />
        public override string TemplateId => "float";
        public MvcHtmlString LeftAddon { get; set; }
        public MvcHtmlString RightAddon { get; set; }
        public string Step { get; set; }
    }
}