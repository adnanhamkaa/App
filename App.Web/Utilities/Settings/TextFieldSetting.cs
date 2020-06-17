using System.Web.Mvc;

namespace App.Web.Utilities.Settings {
    public class TextFieldSetting : FormFieldSetting {
        public TextFieldSetting() : base() {
            Style = TextFieldStyle.Simple;
            MaxLength = 200;
        }

        /// <inheritdoc />
        public override string TemplateId => "text";
        public TextFieldStyle Style { get; set; }
        public string LeftAddon { get; set; }
        public string RightAddon { get; set; }
        public int? MaxLength { get; set; }
    }

    public enum TextFieldStyle {
        Simple = 0,
        Textarea,
        RichText,
        Password,
        Email
    }
}