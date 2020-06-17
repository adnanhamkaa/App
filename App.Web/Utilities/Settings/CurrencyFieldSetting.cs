using System.Web.Mvc;

namespace App.Web.Utilities.Settings
{
    public class CurrencyFieldSetting : FormFieldSetting
    {
        /// <inheritdoc />
        public override string TemplateId => "currency";
        public string LeftAddon { get; set; }
        public string RightAddon { get; set; }
    }
}