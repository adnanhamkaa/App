using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IExcelFileServices {
        FileStream GetWorksheetFileStream(string templatePath);
        IEnumerable<dynamic> ReadExcelData(string path, string sheetName, int headerRow = 1, int firstColdata = 1);
        IEnumerable<dynamic> ReadExcelData(byte[] bytes, string sheetName, int headerRow = 1, int firstColdata = 1);
        IEnumerable<dynamic> ReadExcelData(MemoryStream stream, string sheetName, int headerRow = 1, int firstColdata = 1);
        bool IsFileExists(string templatePath);
    }
}
