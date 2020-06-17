using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace App.Web.Utilities {
    public class PlainJsonStringConverter : Newtonsoft.Json.JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(string);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return reader.Value;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue((string)value);
        }
    }

    public class DttTargetConverter : Newtonsoft.Json.JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(string);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return reader.Value;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            try {
                var number = decimal.Parse(value?.ToString()??"");
                writer.WriteRawValue((string)value);
            } catch (Exception exc) {
                writer.WriteRawValue("\"" + (string)value +"\"");
            }
            
        }
    }
}