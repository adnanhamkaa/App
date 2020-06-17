using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace App.Utilities
{
    public class JsonDotNetResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        public override void ExecuteResult(ControllerContext context) {
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) {
                throw new InvalidOperationException("GET request not allowed");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (this.ContentEncoding != null) {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data == null) {
                return;
            }

            StringWriter sw = new StringWriter();
            using (var writer = new CustomWriter(sw)) {
                JsonSerializer serializer = JsonSerializer.CreateDefault(Settings);
                serializer.Serialize(writer, this.Data);
            }

            response.Write(sw.ToString());

            //response.Write(JsonConvert.SerializeObject(this.Data, Settings));
        }
    }

    public class CustomWriter : Newtonsoft.Json.JsonTextWriter
    {
        public CustomWriter(TextWriter textWriter) : base(textWriter) { }

        public override void WriteValue(string value) {
            base.WriteValue(System.Web.HttpUtility.HtmlEncode(value));
        }

    }
}
