using App.DataAccess;
using App.DataAccess.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Utilities
{
    public static class DttColumnRenderMethod {
        static string[] ROLEADMINIDS => ConfigurationManager.AppSettings["RoleAdminId"].ToString().Split(',');

        public static MvcHtmlString RenderColumnsOption<T>(this HtmlHelper<T> html, Type tableModel) {
            var result = "";

            result = JsonConvert.SerializeObject(GetColumnsOpt(tableModel), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString RenderColumnsHtml<T>(this HtmlHelper<T> html, Type tableModel) {
            StringBuilder result = new StringBuilder("");

            var opts = GetColumnsOpt(tableModel).Where(t => t.targets != "_all").ToList();

            foreach (var opt in opts) {
                result.AppendFormat("<td class=\"{1}\">{0}</td>\n", opt.label, opt.export ? "" : ".notexport");
            }

            return new MvcHtmlString(result.ToString());
        }

        public static List<DttFieldOpt> GetColumnsOpt(Type tableModel, int row = 0, bool isChild = false) {
            var columns = new List<DttFieldOpt>();


            var tableOpt = tableModel.GetCustomAttributes(typeof(DttTableOption), true).FirstOrDefault() as DttTableOption;

            if (!isChild) {
                columns.Add(new DttFieldOpt() {
                    targets = "_all",
                    defaultContent = "-"
                });

                if ((tableOpt?.UseNumberColumn ?? true)) {
                    columns.Add(new DttFieldOpt() {

                        targets = "0",
                        export = tableOpt?.ExportNumberColumn ?? true,
                        label = "No",
                        name = "No",
                        searchable = false,
                        orderable = false

                    });

                    row++;
                }

                var actionrenderName = "renderAction";

                var user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                var isUserAdmin = false;
                if (user != null) {

                    var roles = HttpContext.Current.Cache["roles"] as IEnumerable<ApplicationRole>;

                    if (roles == null) {
                        using (var context = new ApplicationDbContext()) {
                            roles = context.Roles.Include("Actions").Where(t => t.IsActive == true && t.IsDeleted != true && t.IsDraft == false).ToList();
                            HttpContext.Current.Cache["roles"] = roles;
                        }
                    }

                    var actions = roles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList().SelectMany(t => t.Actions).ToList();
                    
                    if (actions.Any(t => t.ActionName == AppActions.Restore_Delete)) {
                        actionrenderName = "defaultrenderAdminAction";
                        isUserAdmin = true;
                    }
                }

                if (tableOpt != null) {
                    if (tableOpt.UseActionColumn) {
                        var logOpt = new DttFieldOpt() {
                            name = tableOpt.ActionColName ?? "Action",
                            render = actionrenderName,//"renderAction",
                            label = tableOpt.ActionColName ?? "Action",
                            targets = "1",
                            searchable = false,
                            orderable = false,
                            export = false
                        };

                        if (!string.IsNullOrEmpty(tableOpt.ActionRenderFunction)) {
                            
                            logOpt.render = tableOpt.ActionRenderFunction;

                            if (isUserAdmin) logOpt.render = tableOpt.ActionAdminRenderFunction;

                        }

                        columns.Add(logOpt);

                        row++;
                    }
                } else {
                    var actionOpt = new DttFieldOpt() {
                        name = "Action",
                        render = actionrenderName,//"renderAction",
                        label = "Action",
                        targets = "1",
                        searchable = false,
                        orderable = false,
                        export = false
                    };

                    columns.Add(actionOpt);

                    row++;
                }

            }

            var props = tableModel.GetProperties();

            foreach (var prop in props) {

                if (prop.DeclaringType != tableModel && isChild) continue;

                if (prop.PropertyType.IsBasicType()) {
                    var opt = ProcessProp(prop, row);
                    if (opt != null) {
                        columns.Add(opt);
                        row++;
                    }
                } else {
                    var dtfieldopt = prop.GetCustomAttributes(typeof(DttFieldAttribute), true).FirstOrDefault() as DttFieldAttribute;
                    var displayAtt = prop.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;

                    if (dtfieldopt != null) {

                        if (dtfieldopt.RenderAllChildren && !isChild) {

                            var children = GetColumnsOpt(prop.PropertyType, row, true);

                            var parentLabel = displayAtt?.Name ?? prop.Name;

                            foreach (var child in children) {
                                
                                child.data = prop.Name + "." + child.data;
                                child.name = child.data;

                                if (dtfieldopt.ShowParentLabel) {
                                    child.label = child.label != null ? string.Format("{0} - {1}", parentLabel, child.label) : null;
                                }
                            }


                            columns.AddRange(children);

                            row = children.Max(t => t.targets.SafeConvert<int>(0)) + 1;

                        } else if (dtfieldopt.RenderChildren != null && dtfieldopt.RenderChildren?.Length > 0) {

                            foreach (var path in dtfieldopt.RenderChildren) {
                                var splitPath = path.Split(':');
                                var childPropInfo = prop.PropertyType.GetPropertyFromPath(splitPath[0]);
                                if (childPropInfo?.PropertyType?.IsBasicType() == true) {
                                    var opt = ProcessProp(childPropInfo, row);
                                    if (opt != null) {
                                        opt.data = prop.Name + "." + splitPath[0];
                                        opt.name = prop.Name + "." + splitPath[0];

                                        var paths = splitPath[0]?.Split('.');

                                        var prefix = "";

                                        if(paths?.Length > 1) {
                                            var tempPath = paths.ToList();
                                            tempPath.RemoveAt(paths.Length - 1);

                                            var parentpathprop = prop.PropertyType.GetPropertyFromPath(string.Join(".",tempPath.ToArray()));

                                            var parentpdisplayAtt = parentpathprop.GetCustomAttribute(typeof(DisplayAttribute));

                                            prefix = parentpathprop?.Name ?? parentpathprop?.Name ?? "";


                                        } else {
                                            prefix = displayAtt?.Name?? prop.Name;
                                        }

                                        if (splitPath.Length > 1) {
                                            opt.label = string.Format(splitPath[1],prefix);
                                        }

                                        columns.Add(opt);
                                        row++;
                                    }
                                }

                            }

                        } else if (!string.IsNullOrEmpty(dtfieldopt.RenderFunction)) {
                            try {
                                var opt = GetPropAttr(prop, row);
                                opt.orderable = false;
                                opt.searchable = false;
                                opt.isnullable = true;

                                opt.render = dtfieldopt.RenderFunction;
                                columns.Add(opt);
                                row++;
                            } catch {

                            }
                        }

                    }

                }

            }
            if (!isChild) {
                if (tableOpt != null) {

                    if (tableOpt.UseStatusColumn) {
                        var statOpt = new DttFieldOpt() {
                            name = tableOpt.StatusColName ?? "#Status",
                            render = "defaultStatusRender",
                            label = tableOpt.StatusColName ?? "Status",
                            targets = row.ToString(),
                            searchable = true,
                            data = "#Status",
                            orderable = true,
                            export = true
                        };

                        if (!string.IsNullOrEmpty(tableOpt.StatusRenderFunction)) {
                            statOpt.render = tableOpt.StatusRenderFunction;
                        }
                        statOpt.SetType(DttFiledType.Status);
                        columns.Add(statOpt);

                        row++;
                    }

                    if (tableOpt.UseLogColumn)
                    {
                        var logOpt = new DttFieldOpt()
                        {
                            name = tableOpt.LogColName ?? "Log",
                            render = "getLog",
                            label = tableOpt.LogColName ?? "Log",
                            targets = row.ToString(),
                            searchable = true,
                            orderable = true
                        };

                        if (string.IsNullOrEmpty(tableOpt.LogRenderFunction))
                        {
                            logOpt.render = tableOpt.LogRenderFunction;
                        }

                        columns.Add(logOpt);
                        row++;
                    }

                    //if (tableOpt.UseActionColumn) {
                    //    var logOpt = new DttFieldOpt() {
                    //        name = tableOpt.ActionColName ?? "Action",
                    //        render = "renderAction",
                    //        label = tableOpt.ActionColName ?? "Action",
                    //        targets = row.ToString(),
                    //        searchable = false,
                    //        orderable = false,
                    //        export = false
                    //    };

                    //    if (!string.IsNullOrEmpty(tableOpt.ActionRenderFunction)) {
                    //        logOpt.render = tableOpt.ActionRenderFunction;
                    //    }

                    //    columns.Add(logOpt);

                    //    row++;
                    //}

                    if (!string.IsNullOrEmpty(tableOpt.CustomColumn))
                    {
                        var cstOpt = new DttFieldOpt()
                        {
                            name = tableOpt.CustomColName,
                            render = tableOpt.CustomColumn,
                            label = tableOpt.CustomColName,
                            targets = row.ToString(),
                            searchable = false,
                            orderable = false,
                            export = false
                        };

                        columns.Add(cstOpt);

                        row++;
                    }
                } else {

                    var statOpt = new DttFieldOpt() {
                        name = "#Status",
                        render = "defaultStatusRender",
                        label = "Status",
                        targets = row.ToString(),
                        searchable = true,
                        data = "#Status",
                        orderable = true,
                        export = true
                    };
                    statOpt.SetType(DttFiledType.Status);
                    columns.Add(statOpt);

                    row++;


                    var logOpt = new DttFieldOpt()
                    {
                        name = "Log",
                        render = "getLog",
                        label = "Log",
                        targets = row.ToString(),
                        searchable = true,
                        orderable = true
                    };

                    columns.Add(logOpt);

                    row++;

                }
            }
            return columns.OrderBy(t => t.targets.SafeDecimalConvert() ?? decimal.MaxValue).ToList();

        }

        public static DttFieldOpt ProcessProp(PropertyInfo prop, int row) {
            var dtfieldopt = prop.GetCustomAttributes(typeof(DttFieldAttribute), true).FirstOrDefault() as DttFieldAttribute;
            var displayAtt = prop.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;

            if (dtfieldopt?.Hidden == true) return null;

            var columnOpt = new DttFieldOpt();

            if (prop.PropertyType.IsBasicType()) {

                columnOpt.data = prop.Name;
                columnOpt.label = prop.Name;
                columnOpt.export = dtfieldopt?.Export ?? true;
                columnOpt.targets = row.ToString();
                columnOpt.name = prop.Name;
                columnOpt.orderable = true;
                columnOpt.searchable = true;


                if (displayAtt != null) {
                    columnOpt.label = displayAtt.GetName();
                }

                if (dtfieldopt != null) {

                    columnOpt.label = dtfieldopt.ColumnName ?? columnOpt.label;
                    columnOpt.targets = row.ToString();
                    columnOpt.export = dtfieldopt.Export;
                    columnOpt.orderable = dtfieldopt.Orderable;
                    columnOpt.searchable = dtfieldopt.Searchable;
                    if (!dtfieldopt.Type.HasValue || dtfieldopt.Type == DttFiledType.Auto) {
                        if (prop.PropertyType == typeof(decimal) ||
                             prop.PropertyType == typeof(decimal?) ||
                             prop.PropertyType == typeof(int) ||
                             prop.PropertyType == typeof(int?) ||
                             prop.PropertyType == typeof(double) ||
                             prop.PropertyType == typeof(double?)) {



                            columnOpt.render = @"datatableNumber()";
                            columnOpt.SetType(DttFiledType.Number);
                        } else if (prop.PropertyType == typeof(DateTime)) {
                            
                            columnOpt.render = @"datatableDate('DD MMMM YYYY')";
                            columnOpt.SetType(DttFiledType.Date);
                        } else if (prop.PropertyType == typeof(DateTime?)) {

                            columnOpt.render = @"datatableDate('DD MMMM YYYY')";
                            columnOpt.SetType(DttFiledType.NullableDateTime);
                        } else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?)) {

                            columnOpt.render = @"datatableBool()";
                            columnOpt.SetType(DttFiledType.NullableDateTime);

                        } else {
                            columnOpt.SetType(DttFiledType.String);
                        }
                    } else {
                        columnOpt.SetType(dtfieldopt.Type.Value);
                    }

                    if (dtfieldopt.RenderFunction != null) {

                        columnOpt.render = dtfieldopt.RenderFunction;

                        if(!columnOpt.render.StartsWith("datatableDate") && !columnOpt.render.StartsWith("datatableNumber") && !columnOpt.render.StartsWith("datatableBool")) {

                            columnOpt.searchable = false;
                            columnOpt.orderable = false;
                        }
                    } else {

                        switch (dtfieldopt.Type) {
                            case DttFiledType.String:
                                break;
                            case DttFiledType.Date:
                                columnOpt.render = @"datatableDate('DD MMMM YYYY')";
                                break;
                            case DttFiledType.DateTime:
                                columnOpt.render = @"datatableDate('DD MMMM YYYY HH:mm')";
                                break;
                            case DttFiledType.Number:
                                columnOpt.render = @"datatableNumber()";
                                break;
                            case DttFiledType.Bool:
                                columnOpt.render = @"datatableBool()";
                                break;
                            default:
                                break;
                        }

                    }

                } else {
                    if (prop.PropertyType == typeof(decimal) ||
                        prop.PropertyType == typeof(decimal?) ||
                        prop.PropertyType == typeof(int) ||
                        prop.PropertyType == typeof(int?) ||
                        prop.PropertyType == typeof(double) ||
                        prop.PropertyType == typeof(double?)) {

                        columnOpt.render = @"datatableNumber()";
                        columnOpt.SetType(DttFiledType.Number);

                    } else if (prop.PropertyType == typeof(DateTime)) {

                        columnOpt.render = @"datatableDate('DD MMMM YYYY')";
                        columnOpt.SetType(DttFiledType.Date);

                    } else if (prop.PropertyType == typeof(DateTime?)) {

                        columnOpt.render = @"datatableDate('DD MMMM YYYY')";
                        columnOpt.SetType(DttFiledType.NullableDateTime);

                    } else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?)) {

                        columnOpt.render = @"datatableBool()";
                        columnOpt.SetType(DttFiledType.Bool);

                    } else {

                        columnOpt.SetType(DttFiledType.String);

                    }
                }
                return columnOpt;
            } else {
                return null;
            }
        }

        public static DttFieldOpt GetPropAttr(PropertyInfo prop, int row) {
            var dtfieldopt = prop.GetCustomAttributes(typeof(DttFieldAttribute), true).FirstOrDefault() as DttFieldAttribute;
            var displayAtt = prop.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;

            if (dtfieldopt?.Hidden == true) return null;
            if (dtfieldopt == null) return null;

            var columnOpt = new DttFieldOpt();

            columnOpt.data = prop.Name;
            columnOpt.label = prop.Name;
            columnOpt.export = dtfieldopt?.Export ?? true;
            columnOpt.targets = row.ToString();
            columnOpt.name = prop.Name;
            columnOpt.orderable = true;
            columnOpt.searchable = true;

            if (displayAtt != null) {
                columnOpt.label = displayAtt.GetName();
            }

            columnOpt.label = dtfieldopt.ColumnName ?? columnOpt.label;
            columnOpt.targets = row.ToString();
            columnOpt.export = dtfieldopt.Export;
            columnOpt.orderable = dtfieldopt.Orderable;
            columnOpt.searchable = dtfieldopt.Searchable;

            return columnOpt;
        }

        public static MvcHtmlString RenderHtmlColumns<T>(this HtmlHelper<T> html, Type tableModel) {
            var result = "";



            return new MvcHtmlString(result);
        }
    }
}