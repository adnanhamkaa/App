using System.Collections.Generic;
using JrzAsp.Lib.RazorTools;

namespace App.Web.Utilities.Settings
{
    public class UploadFieldSetting : FormFieldSetting {

        private string _folderPath;
        private IEnumerable<string> _validFileExtensions;
        private IEnumerable<string> _validMimeTypes;

        /// <inheritdoc />
        public override string TemplateId => "upload";

        public string UploadUrl => UrlHelperGlobal.Self.Action("Upload", "FileManager", new {area = ""});

        public IEnumerable<string> ValidFileExtensions {
            get { return _validFileExtensions ?? new string[0]; }
            set { _validFileExtensions = value; }
        }

        public IEnumerable<string> ValidMimeTypes {
            get { return _validMimeTypes ?? new string[0]; }
            set { _validMimeTypes = value; }
        }

        public int? MaxBytes { get; set; }

        public string FolderPath {
            get { return _folderPath; }
            set { _folderPath = value?.Trim('/', '\\', ' ', '\t', '\r', '\n'); }
        }
    }
}