using App.Web.Services.Contracts;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace App.Web.Services.Repositories {
    public class ExcelFileServices : ServiceBase, IExcelFileServices {

        public FileStream GetWorksheetFileStream(string templatePath) {
            var pathdir = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var path = Path.Combine(pathdir, templatePath);

            return File.OpenRead(path);
        }

        public bool IsFileExists(string templatePath) {
            var pathdir = ConfigurationManager.AppSettings["doctemplate"].ToString();

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var path = Path.Combine(pathdir, templatePath);

            return File.Exists(path);
        }
        
        public IEnumerable<dynamic> ReadExcelData(string path, string sheetName,int headerRow = 1,int firstColdata = 1) {

            var pathdir = path;

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }
            
            var result = new List<dynamic>();

            using (var templateStream = File.OpenRead(pathdir)) {

                using (var package = new ExcelPackage(templateStream)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    var rowCnt = worksheet.Dimension.End.Row;
                    var colCnt = worksheet.Dimension.End.Column + 1;

                    for (int row = headerRow+1; row <= rowCnt; row++) {
                        dynamic data = new System.Dynamic.ExpandoObject();

                        for (int col = firstColdata; col < colCnt; col++) {
                            var column = worksheet.Cells[headerRow, col].Value;
                            if (column != null) {
                                Regex rgx = new Regex("[^a-zA-Z0-9]");
                                var colname = rgx.Replace(column.ToString(), "").Replace(" ", "");
                                
                                ((IDictionary<string, object>)data)[colname] = worksheet.Cells[row, col].Value;
                            }
                        }

                        result.Add(data);

                    }

                }

            }

            return result;

        }

        public IEnumerable<dynamic> ReadExcelData(string path, string sheetName, int headerRow = 1, int firstColdata = 1, bool peredictEndOfData = false, int keyCol = 1) {

            var pathdir = path;

            if (pathdir.StartsWith("~")) {
                pathdir = HttpContext.Current.Server.MapPath(pathdir);
            }

            var result = new List<dynamic>();

            using (var templateStream = File.OpenRead(pathdir)) {

                using (var package = new ExcelPackage(templateStream)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    var rowCnt = worksheet.Dimension.End.Row;
                    var colCnt = worksheet.Dimension.End.Column + 1;

                    for (int row = headerRow + 1; row <= rowCnt; row++) {
                        dynamic data = new System.Dynamic.ExpandoObject();

                        for (int col = firstColdata; col < colCnt; col++) {
                            var column = worksheet.Cells[headerRow, col].Value;
                            if (column != null) {
                                Regex rgx = new Regex("[^a-zA-Z0-9]");
                                var colname = rgx.Replace(column.ToString(), "").Replace(" ", "");

                                ((IDictionary<string, object>)data)[colname] = worksheet.Cells[row, col].Value;
                            }
                        }

                        result.Add(data);

                    }

                }

            }

            return result;

        }


        public IEnumerable<dynamic> ReadExcelData(byte[] bytes, string sheetName, int headerRow = 1, int firstColdata = 1) {
            
            var result = new List<dynamic>();

            using (var templateStream = new MemoryStream(bytes)) {

                using (var package = new ExcelPackage(templateStream)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    var rowCnt = worksheet.Dimension.End.Row;
                    var colCnt = worksheet.Dimension.End.Column + 1;

                    for (int row = headerRow + 1; row <= rowCnt; row++) {
                        dynamic data = new System.Dynamic.ExpandoObject();

                        for (int col = firstColdata; col < colCnt; col++) {
                            var column = worksheet.Cells[headerRow, col].Value;
                            if (column != null) {
                                Regex rgx = new Regex("[^a-zA-Z0-9]");
                                var colname = rgx.Replace(column.ToString(), "").Replace(" ", "");

                                ((IDictionary<string, object>)data)[colname] = worksheet.Cells[row, col].Value;
                            }
                        }

                        result.Add(data);

                    }

                }

            }

            return result;

        }

        public IEnumerable<dynamic> ReadExcelData(MemoryStream stream, string sheetName, int headerRow = 1, int firstColdata = 1) {

            var result = new List<dynamic>();

            using (var templateStream = stream) {

                using (var package = new ExcelPackage(templateStream)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    var rowCnt = worksheet.Dimension.End.Row;
                    var colCnt = worksheet.Dimension.End.Column + 1;

                    for (int row = headerRow + 1; row <= rowCnt; row++) {
                        dynamic data = new System.Dynamic.ExpandoObject();

                        for (int col = firstColdata; col < colCnt; col++) {
                            var column = worksheet.Cells[headerRow, col].Value;
                            if (column != null) {
                                Regex rgx = new Regex("[^a-zA-Z0-9]");
                                var colname = rgx.Replace(column.ToString(), "").Replace(" ", "");

                                ((IDictionary<string, object>)data)[colname] = worksheet.Cells[row, col].Value;
                            }
                        }

                        result.Add(data);

                    }

                }

            }

            return result;

        }
        
    }
}