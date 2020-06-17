using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
namespace App.Web.Models {
    public class WordCheckBox {
        public string Value { get; set; }
        public string Label { get; set; }
        
        public WordCheckBox() {

        }

        public WordCheckBox(string label, string value) {
            this.Value = value;
            this.Label = label;
        }

    }

    public static class ExtendWordCheckBox {
        public static List<Run> GetCheckBoxes(this List<WordCheckBox> checkboxes, List<string> checkedValues) {
            var result = new List<Run>();
            checkedValues = checkedValues ?? new List<string>();
            string uncheck = "00A3";
            string check = "0052";

            foreach (var item in checkboxes) {
                var isChecked = checkedValues.Any(t => t == item.Value);

                var newrun = new Run();

                newrun.Append(new SymbolChar() {
                    Font = "Wingdings 2",
                    Char = new HexBinaryValue(isChecked ? check : uncheck)
                });


                newrun.Append(new Text(item.Label + "  ") {
                    Space = SpaceProcessingModeValues.Preserve
                });
                result.Add(newrun);
                //result.Insert(0, newrun);
            }


            return result;
        }

        public static List<Run> GetCheckBoxes(this List<WordCheckBox> checkboxes, string checkedValue) {
            var result = new List<Run>();

            string uncheck = "00A3";
            string check = "0052";

            foreach (var item in checkboxes) {
                var isChecked = checkedValue ==  item.Value;

                var newrun = new Run();
                
                newrun.Append(new SymbolChar() {
                    Font = "Wingdings 2",
                    Char = new HexBinaryValue(isChecked ? check : uncheck)
                });

                RunProperties runProp = new RunProperties();
                FontSize fontSize = new FontSize { Val = "18" };

                runProp.Append(fontSize);
                newrun.Append(runProp);

                newrun.Append(new Text(item.Label + "  ") {
                    Space = SpaceProcessingModeValues.Preserve
                });
                //result.Insert(0, newrun);
                result.Add(newrun);
            }


            return result;
        }
    }

}