using DocumentFormat.OpenXml.Wordprocessing;
using App.DataAccess;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Web.Models
{
    public class WordReplacement {
        public string TextToReplace { get; set; }
        public string ReplacementText { get; set; }
        public bool IsCheckBox { get; set; }
        public bool MatchWholeText { get; set; }
        public List<Run> Checkboxes { get; set; }
        public Run Run { get; set; }
        public bool UseRun { get; set; }
        
        public WordReplacement() {
            this.MatchWholeText = false;
            this.UseRun = false;
        }

        public WordReplacement(string toReplace, string replacement) {
            this.MatchWholeText = false;
            this.TextToReplace = toReplace;
            this.ReplacementText = replacement;
            this.UseRun = false;
        }

        public static List<WordReplacement> GetLogs(FormModelBase vm) {

            var result = new List<WordReplacement>();

            DateTime? date = DateTime.Now;//!string.IsNullOrEmpty(vm.UpdatedBy?.Trim()) ? vm.UpdatedDate:vm.CreatedDate;
            

            if (vm != null) {

                var by = WebHelper.GetUserByEmail(vm.UpdatedBy ?? vm.CreatedBy);//!string.IsNullOrEmpty(vm.UpdatedBy?.Trim())? vm.UpdatedBy:vm.CreatedBy;

                result.Add(new WordReplacement(":Created;", (vm.CreatedDate.ToString("dd MMM yyyy") ?? "")));
                result.Add(new WordReplacement("#Created", (vm.CreatedDate.ToString("dd MMM yyyy") ?? "")));
                result.Add(new WordReplacement("CDate", (vm.CreatedDate.ToString("dd MMM yyyy") ?? "")));
                result.Add(new WordReplacement(":CreatedBy;", by?.FullName.ToPascalCase() ?? ""));
                result.Add(new WordReplacement(":CreatedB;", by?.FullName.ToPascalCase() ?? ""));
                result.Add(new WordReplacement("#CreatedBy", by?.FullName.ToPascalCase() ?? ""));
                result.Add(new WordReplacement("pembuat", by?.FullName.ToPascalCase() ?? ""));
                result.Add(new WordReplacement("FUser", by?.FullName?.ToPascalCase() ?? ""));
                result.Add(new WordReplacement("FTitle", by?.Title?.ToPascalCase() ?? ""));
                result.Add(new WordReplacement(":Time;", (vm.CreatedDate.ToString("HH:mm") ?? "")));
                result.Add(new WordReplacement("#Time", (vm.CreatedDate.ToString("HH:mm") ?? "")));
                result.Add(new WordReplacement("FCTime", (vm.CreatedDate.ToString("HH:mm") ?? "")));

            }

            ApplicationDbContext context = new ApplicationDbContext();

            //var signature = Mapper.Map<SignatureForm>(context.Signature.Where(t => t.IsDeleted != true && t.IsDraft != true).FirstOrDefault());

            //result.Add(new WordReplacement("FDiperiksa", signature?.Diperiksa ?? ""));
            //result.Add(new WordReplacement("FDisetujui", signature?.Disetujui ?? ""));
            //result.Add(new WordReplacement("FKadiv", signature?.KepalaDivisi ?? ""));
            //result.Add(new WordReplacement("FDivisi", signature?.Divisi ?? ""));

            return result;

        }
        
        //public static List<WordReplacement> GetLogsFromCurrentuser() {
            
        //    DateTime date = DateTime.Now;//!string.IsNullOrEmpty(vm.UpdatedBy?.Trim()) ? vm.UpdatedDate:vm.CreatedDate;
        //    var result = new List<WordReplacement>();
            
        //    var by = CommonHelper.GetUser();//!string.IsNullOrEmpty(vm.UpdatedBy?.Trim())? vm.UpdatedBy:vm.CreatedBy;

        //    result.Add(new WordReplacement(":Created;", (date.ToStringIndonesia("dd MMM yyyy") ?? "")));
        //    result.Add(new WordReplacement("#Created", (date.ToStringIndonesia("dd MMM yyyy") ?? "")));
        //    result.Add(new WordReplacement("CDate", (date.ToStringIndonesia("dd MMM yyyy") ?? "")));
        //    result.Add(new WordReplacement(":CreatedBy;", by?.FullName.ToPascalCase() ?? ""));
        //    result.Add(new WordReplacement(":CreatedB;", by?.FullName.ToPascalCase() ?? ""));
        //    result.Add(new WordReplacement("#CreatedBy", by?.FullName.ToPascalCase() ?? ""));
        //    result.Add(new WordReplacement("pembuat", by?.FullName.ToPascalCase() ?? ""));
        //    result.Add(new WordReplacement("FUser", by?.FullName?.ToPascalCase() ?? ""));
        //    result.Add(new WordReplacement(":Time;", (date.ToStringIndonesia("HH:mm") ?? "")));
        //    result.Add(new WordReplacement("#Time", (date.ToStringIndonesia("HH:mm") ?? "")));
        //    result.Add(new WordReplacement("FCTime", (date.ToStringIndonesia("HH:mm") ?? "")));
            
        //    ApplicationDbContext context = new ApplicationDbContext();

        //    //var signature = Mapper.Map<SignatureForm>(context.Signature.Where(t => t.IsDeleted != true && t.IsDraft != true).FirstOrDefault());

        //    //result.Add(new WordReplacement("FDiperiksa", signature?.Diperiksa ?? ""));
        //    //result.Add(new WordReplacement("FDisetujui", signature?.Disetujui ?? ""));
        //    //result.Add(new WordReplacement("FKadiv", signature?.KepalaDivisi ?? ""));
        //    //result.Add(new WordReplacement("FDivisi", signature?.Divisi ?? ""));

        //    return result;

        //}


        public static List<WordReplacement> BreakByChar(string text, int charcount, string replaceformat = "{0}") {

            var result = new List<WordReplacement>();

            for (int i = 0; i < charcount; i++) {
                if (text.Length - 1 >= i) {
                    result.Add(new WordReplacement() {
                        MatchWholeText = true,
                        TextToReplace = string.Format(replaceformat, i + 1),
                        ReplacementText = text[i].ToString()
                    });
                } else {
                    result.Add(new WordReplacement() {
                        MatchWholeText = true,
                        TextToReplace = string.Format(replaceformat, i + 1),
                        ReplacementText = string.Empty
                    });
                }
            }

            return result;

        }

    }

    public static class WordReplacementHelper {
        public static void AddOrUpdate(this List<WordReplacement> list,WordReplacement replacement) {

            var index = list.Where(t => t.TextToReplace == replacement.TextToReplace).FirstOrDefault();

            if (index != null) index = replacement;
            else list.Add(replacement);

        }
    }
}

