using App.Utilities.Model;
using App.Web.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IPdfServices : IServiceBase {
        FileModel GetPdf(string actionUrl, string fileName, PdfPageSize pageSize = PdfPageSize.A4, PdfPageOrientation orientation = PdfPageOrientation.Portrait);
        FileModel GetPdfFromHtmlString(string html, string fileName, PdfPageSize pageSize = PdfPageSize.A4, PdfPageOrientation orientation = PdfPageOrientation.Portrait);
    }
}
