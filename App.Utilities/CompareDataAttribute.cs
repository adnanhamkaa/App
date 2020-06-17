using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public class CompareDataAttribute : Attribute {
        public bool Excluded { get; set; }
    }
}