using System.Web.Mvc;

namespace App.Web.Utilities.Settings {
    public class IntFieldSetting : FormFieldSetting {
        /// <inheritdoc />
        public override string TemplateId => "int";
        public MvcHtmlString LeftAddon { get; set; }
        public MvcHtmlString RightAddon { get; set; }
    }
}