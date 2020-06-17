using System.Web.Mvc;

namespace App.Web.Utilities.Settings
{
    public class DoubleFieldSetting : FormFieldSetting
    {
        /// <inheritdoc />
        public override string TemplateId => "double";
        public string LeftAddon { get; set; }
        public string RightAddon { get; set; }
        public decimal? MaxValue { get; set; }
    }
}