using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace App.Utilities.Model
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string Mime { get; set; }
        public byte[] Bytes { get; set; }
        public string Url { get; set; }


        public const string MIME_DOCX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string MIME_XLS = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string MIME_PDF = "application/pdf";
        public const string CSV_TEXT = "text/csv";

        internal IEnumerable AsEnumerable() {
            throw new NotImplementedException();
        }

        public static FileModel GetFromPostedFile(HttpPostedFileBase posted) {

            if (posted == null) return null;

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(posted.InputStream)) {
                fileData = binaryReader.ReadBytes(posted.ContentLength);

                return new FileModel() {
                    Bytes = fileData,
                    FileName = Path.GetFileName(posted.FileName),
                    Mime = MimeMapping.GetMimeMapping(posted.FileName)
                };

            }

        }
    }
}
