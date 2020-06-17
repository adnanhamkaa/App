using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    [AttributeUsage(AttributeTargets.Property)]
    public class DttFieldAttribute : Attribute {
        public bool Hidden { get; set; }
        public string ColumnName { get; set; }
        public string[] RenderChildren { get; set; }
        public bool RenderAllChildren { get; set; }
        public string RenderFunction { get; set; }
        public bool ShowParentLabel { get; set; }
        public DttFiledType? Type { get; set; }
        public bool Export { get; set; }
        public bool Orderable { get; set; }
        public bool Searchable { get; set; }
        public bool isCustom { get; set; }

        public DttFieldAttribute(bool hidden = false,
            string columnName = null,
            bool renderChild = false,
            bool overrideRender = false,
            string renderFunction = null,
            DttFiledType type = DttFiledType.Auto,
            string[] renderChildren = null,
            bool export = true,
            bool orderable = true,
            bool searchable = true,
            bool showParentLabel = false,
            bool renderAllChildren = false,
            bool isCustom = false) {

            this.Hidden = hidden;
            this.ColumnName = columnName;
            this.RenderFunction = renderFunction;
            this.Type = type;
            this.RenderChildren = renderChildren;
            this.Export = export;
            this.Orderable = orderable;
            this.Searchable = searchable;
            this.RenderAllChildren = renderAllChildren;
            this.ShowParentLabel = showParentLabel;
            this.isCustom = isCustom;
        }
        
    }

    public enum DttFiledType {
        String,
        Date,
        DateTime,
        NullableDateTime,
        Number,
        Status,
        Auto,
        Bool
    }
    

    public class DttFieldOpt {
        public string defaultContent { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string data { get; set; }
        [JsonConverter(typeof(DttTargetConverter))]
        public string targets { get; set; }
        [JsonConverter(typeof(PlainJsonStringConverter))]
        public string render { get; set; }
        public bool export { get; set; }
        public bool isnullable { get; set; }
        public bool orderable { get; set; }
        public bool searchable { get; set; }
        public string type { get; set; }

        public void SetType(DttFiledType type) {
            switch (type) {
                case DttFiledType.String:
                    this.type = "string";
                    break;
                case DttFiledType.Date:
                    this.type = "date";
                    break;
                case DttFiledType.DateTime:
                    this.type = "datetime";
                    break;
                case DttFiledType.Number:
                    this.type = "number";
                    break;
                case DttFiledType.Status:
                    this.type = "status";
                    break;
                case DttFiledType.Bool:
                    this.type = "bool";
                    break;
                case DttFiledType.NullableDateTime:
                    this.type = "nullabledatetime";
                    break;
                default:
                    this.type = "string";
                    break;
            }
        }

    }
}