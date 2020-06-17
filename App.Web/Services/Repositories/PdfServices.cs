using App.Utilities.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace App.Web.Services.Repositories {
    public class PdfServices : ServiceBase, IPdfServices {

        public string UrlSchema => ConfigurationManager.AppSettings["UrlSchema"];

        public FileModel GetPdf(string actionUrl, string fileName, PdfPageSize pageSize = PdfPageSize.A4, PdfPageOrientation orientation = PdfPageOrientation.Portrait) {

            var result = new FileModel() {
                FileName = fileName,
                Mime = MimeMapping.GetMimeMapping(fileName)
            };
            
            HttpClient client = new HttpClient();

            var req = HttpContext.Current.Request;

            string url = (UrlSchema == null ? (req.Url.Scheme + "://" + req.Url.Authority + req.ApplicationPath.TrimEnd('/')) : UrlSchema) + actionUrl;
            //string url = actionUrl;
            //File.WriteAllText("C:\\Log\\pdf_serive" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt", url);
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = orientation;

            converter.Options.MaxPageLoadTime = 200;
            converter.Options.MarginTop = 5;

            System.Drawing.Font font = new System.Drawing.Font("Arial", 1);

            //PdfTextSection footer = new PdfTextSection(0, 10, "Dokumen ini adalah surat perintah bayar yang sah walaupun tanpa tanda tangan pejabat ybs, dicetak dari system User Purchase PDSI", font);
            //converter.Footer.Add(footer);
            // create a new pdf document converting an url

            PdfDocument doc = converter.ConvertUrl(url);

            // save pdf document
            var byteresult = doc.Save();

            result.Bytes = byteresult;

            // close pdf document
            doc.Close();

            return result;

        }

        public FileModel GetPdfFromHtmlString(string html, string fileName, PdfPageSize pageSize = PdfPageSize.A4, PdfPageOrientation orientation = PdfPageOrientation.Portrait) {

            var result = new FileModel() {
                FileName = fileName,
                Mime = MimeMapping.GetMimeMapping(fileName)
            };

            HttpClient client = new HttpClient();

            var req = HttpContext.Current.Request;

            //string url = req.Url.Scheme + "://" + req.Url.Authority + req.ApplicationPath.TrimEnd('/') + actionUrl;
            ////string url = actionUrl;
            //File.WriteAllText("C:\\Log\\pdf_serive" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt", url);
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = orientation;

            converter.Options.MaxPageLoadTime = 200;
            converter.Options.MarginTop = 5;

            System.Drawing.Font font = new System.Drawing.Font("Arial", 1);

            //PdfTextSection footer = new PdfTextSection(0, 10, "Dokumen ini adalah surat perintah bayar yang sah walaupun tanpa tanda tangan pejabat ybs, dicetak dari system User Purchase PDSI", font);
            //converter.Footer.Add(footer);
            // create a new pdf document converting an url

            PdfDocument doc = converter.ConvertHtmlString(html);

            // save pdf document
            var byteresult = doc.Save();

            result.Bytes = byteresult;

            // close pdf document
            doc.Close();

            return result;

        }
    }
}