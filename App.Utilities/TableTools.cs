using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public class TableTools {
        public Func<object, IHtmlString> Tools { get; set; }
        public string tableName { get; set; }
    }
}