namespace App.Web.Utilities.Settings {
    public class StringPairListFieldSetting : FormFieldSetting {
        /// <inheritdoc />
        public override string TemplateId => "string-pair-list";
        
        public string LeftLabel { get; set; }
        public string LeftInputName { get; set; }

        public string RightLabel { get; set; }
        public string RightInputName { get; set; }
    }
}