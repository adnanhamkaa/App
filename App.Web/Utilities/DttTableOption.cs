using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    [AttributeUsage(AttributeTargets.Class)]
    public class DttTableOption : Attribute {
        [JsonConverter(typeof(PlainJsonStringConverter))]
        public string ActionRenderFunction { get; set; }
        [JsonConverter(typeof(PlainJsonStringConverter))]
        public string ActionAdminRenderFunction { get; set; }
        public bool UseSoftDelete { get; set; }
        [JsonConverter(typeof(PlainJsonStringConverter))]
        public string StatusRenderFunction { get; set; }
        [JsonConverter(typeof(PlainJsonStringConverter))]
        public string LogRenderFunction { get; set; }
        public bool UseStatusColumn { get; set; }
        public bool UseLogColumn { get; set; }
        public bool UseActionColumn { get; set; }
        public string StatusColName { get; set; }
        public string ActionColName { get; set; }
        public string LogColName { get; set; }
        public bool UseCreatedCol { get; set; }
        public bool UseUpdatedCol { get; set; }
        public bool UseNumberColumn { get; set; }
        public bool ExportNumberColumn { get; set; }
        public string CustomColumn { set; get; }
        public string CustomColName { set; get; }

        public DttTableOption(
            string actionRenderFunction = null,
            string statusRenderFunction = null,
            string logRenderFunction = null,
            bool useSoftDelete = true,
            bool useStatusColumn = true,
            bool useLogColumn = false,
            bool useCreatedColumn = true,
            bool useUpdatedColumn = true,
            bool useActionColumn = true,
            string statusColName = "Status",
            string actionColName = "Actions",
            string logColName = "Log",
            bool useNumberColumn = true,
            bool exportNumberColumn = true,
            string customColumn = null,
            string customColName = "",
            string actionAdminRenderFunction = null
            ) {

            this.ActionRenderFunction = actionRenderFunction;
            this.ActionAdminRenderFunction = actionAdminRenderFunction ?? actionRenderFunction;
            this.UseSoftDelete = useSoftDelete;
            this.StatusRenderFunction = statusRenderFunction;
            this.LogRenderFunction = logRenderFunction;
            this.UseStatusColumn = useStatusColumn;
            this.UseLogColumn = useLogColumn;
            this.UseActionColumn = useActionColumn;
            this.StatusColName = statusColName;
            this.ActionColName = actionColName;
            this.LogColName = logColName;
            this.UseNumberColumn = useNumberColumn;
            this.ExportNumberColumn = exportNumberColumn;
            this.CustomColumn = customColumn;
            this.CustomColName = customColName;
            this.UseCreatedCol = useCreatedColumn;
            this.UseUpdatedCol = useUpdatedColumn;
        }

    }
}