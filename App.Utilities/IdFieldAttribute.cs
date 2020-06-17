using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {

    [AttributeUsage(AttributeTargets.Property)]
    public class IdFieldAttribute : Attribute {
        public string FieldName { get; set; }
     
        public IdFieldAttribute(string fieldName) {
            this.FieldName = fieldName;
        }
    }
}