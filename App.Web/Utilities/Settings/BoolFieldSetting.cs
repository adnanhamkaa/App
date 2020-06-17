namespace App.Web.Utilities.Settings {
    public class BoolFieldSetting : FormFieldSetting {
        public BoolFieldSetting() {
            YesLabel = "Ya";
            NoLabel = "Tidak";
            NullLabel = "-n/a-";
        }

        /// <inheritdoc />
        public override string TemplateId => "bool";

        public string YesLabel { get; set; }
        public string NoLabel { get; set; }
        public bool AllowNull { get; set; }
        public string NullLabel { get; set; }
        public bool? DefaultUnchecked { get; set; }
    }
}