using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public class KeyValue {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValue() { }
        public KeyValue(string key, string value) {
            this.Key = key;
            this.Value = value;
        }
    }
}