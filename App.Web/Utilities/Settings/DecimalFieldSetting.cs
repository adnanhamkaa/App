using System.Web.Mvc;

namespace App.Web.Utilities.Settings
{
    public class DecimalFieldSetting : FormFieldSetting
    {
        /// <inheritdoc />
        public override string TemplateId => "decimal";
        public string LeftAddon { get; set; }
        public string RightAddon { get; set; }
        public decimal? MaxValue { get; set; }
    }
}