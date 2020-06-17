using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Model {
    public class KeyValueStore : ModelBase {

        public KeyValueStore() : base() { }
        public KeyValueStore(string key,string value, string value2 = null, string desc = null) : base() {
            Key = key;
            Value = value;
            Value2 = value2;
            Desc = desc;
        }

        public const string RootPath = nameof(RootPath);
        public const string KoreksiTradingLastUpdate = nameof(KoreksiTradingLastUpdate);
        public const string NARLastUpdate = nameof(NARLastUpdate);
        public const string DiscountLevyLastUpdate = nameof(DiscountLevyLastUpdate);
        public const string InvalidTradingIDLastUpdate = nameof(InvalidTradingIDLastUpdate);
        public const string InvalidTradingAccountLastUpdate = nameof(InvalidTradingAccountLastUpdate);


        public string Key { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string Desc { get; set; }
    }
}
