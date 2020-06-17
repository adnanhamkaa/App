using App.DataAccess;
using App.DataAccess.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Services.Repositories {
    public class KeyValueStoreServices : ServiceBase, IKeyValueStoreServices {
        public KeyValueStoreServices(ApplicationDbContext context) {
            this.context = context;
        }

        public List<KeyValueStore> GetValuesByKey(string key) {
            return context.KeyValueStore.Where(t => t.Key == key).ToList() ?? new List<KeyValueStore>();
        }

        public List<KeyValueStore> UpsertKeyValues(string key,List<KeyValueStore> data) {
            context.KeyValueStore.RemoveRange(context.KeyValueStore.Where(t => t.Key == key).ToList());
            data = data.Select(t => {

                t.Init();
                t.SetCreated();
                t.Key = key;
                return t;
            }).ToList();
            context.KeyValueStore.AddRange(data);

            context.SaveChanges();

            return data;
        }

        public KeyValueStore UpdateValue(string key,string value) {
            var curr = context.KeyValueStore.Where(t => t.Key == key).FirstOrDefault();

            if(curr != null) {
                curr.SetUpdated();
                curr.Value = value;
            } else {
                curr = new KeyValueStore();
                curr.Init();
                curr.SetCreated();

                curr.Key = key;
                curr.Value = value;

                context.KeyValueStore.Add(curr);
            }

            context.SaveChanges();

            return curr;
        }

        public KeyValueStore UpdateValue(string key, string value, string value2) {
            var curr = context.KeyValueStore.Where(t => t.Key == key).FirstOrDefault();

            if (curr != null) {
                curr.SetUpdated();
                curr.Value = value;
                curr.Value2 = value2;
            } else {
                curr = new KeyValueStore();
                curr.Init();
                curr.SetCreated();

                curr.Key = key;
                curr.Value = value;
                curr.Value2 = value2;

                context.KeyValueStore.Add(curr);
            }

            context.SaveChanges();

            return curr;
        }

        public KeyValueStore UpdateValue(string key, string value, string value2, string desc) {
            var curr = context.KeyValueStore.Where(t => t.Key == key).FirstOrDefault();

            if (curr != null) {
                curr.SetUpdated();
                curr.Value = value;
                curr.Value2 = value2;
                curr.Desc = desc;

            } else {
                curr = new KeyValueStore();
                curr.Init();
                curr.SetCreated();

                curr.Key = key;
                curr.Value = value;
                curr.Value2 = value2;
                curr.Desc = desc;

                context.KeyValueStore.Add(curr);
            }

            context.SaveChanges();

            return curr;
        }

        public KeyValueStore GetValueByKey(string key) {
            return context.KeyValueStore.Where(t => t.Key == key).FirstOrDefault();
        }

    }
}