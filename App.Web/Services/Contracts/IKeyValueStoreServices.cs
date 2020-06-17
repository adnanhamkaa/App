using App.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IKeyValueStoreServices : IServiceBase {
        List<KeyValueStore> GetValuesByKey(string key);
        List<KeyValueStore> UpsertKeyValues(string key, List<KeyValueStore> data);
        KeyValueStore UpdateValue(string key, string value);
        KeyValueStore UpdateValue(string key, string value, string value2);
        KeyValueStore UpdateValue(string key, string value, string value2, string desc);
        KeyValueStore GetValueByKey(string key);
    }
}
