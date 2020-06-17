using AutoMapper;
using App.DataAccess;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using App.DataAccess.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.IO;
using Elmah;
using App.Utilities.Model;
using Ninject;

namespace App.Web.Services.Repositories {
    public class ServiceBase : IServiceBase {
        
        public ApplicationDbContext context { get; set; }
        protected const string STANDARD_FORMAT_DATE = "dd MMMM yyyy";
        protected const string DAILY_FOLDER_NAME = "Daily Result";
        protected string[] ROLEADMINIDS => ConfigurationManager.AppSettings["RoleAdminId"].ToString().Split(',');
        public DateTime? Today => DateTime.Today.Date;
        public DateTime? Tomorrow => DateTime.Today.Date.AddWorkDays(1);
        public DateTime? Yesterday => DateTime.Today.Date.AddWorkDays(-1);

        [Inject]
        public void Resolve(ApplicationDbContext dbcontext) {
            if(this.context == null)
                this.context = dbcontext;
        }

        public void Commit() {
            context.SaveChanges();
        }

        public DbContextTransaction BeginTran() {
            return this.context.Database.BeginTransaction();
        }

        public IQueryable<T> GetPaging<T>(IQueryable<T> filtered, DttRequestForm form) where T : ModelBase {

            if (form.order?.Count > 0) {

                foreach (var item in form.order) {


                    item.colname = form.columns[item.column].data;

                    if (item.colname == "Log") {
                        filtered = filtered
                            .OrderBy("CreatedDate " + item.dir);
                        if (item.dir == "asc")
                            filtered = filtered.OrderBy(t => t.UpdatedDate ?? t.CreatedDate);
                        else
                            filtered = filtered.OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
                    } else {

                        filtered = filtered.OrderBy(item.colname + " " + item.dir);
                    }
                }

            } else {

                filtered = filtered.OrderByDescending(t => t.CreatedDate).OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            }

            if (form.length.HasValue) {
                return filtered.Skip((form.start / form.length.Value) * form.length.Value)
                .Take(form.length.Value);
            } else {
                return filtered;
            }
        }
        
        public IEnumerable<T> GetPaging<T>(IEnumerable<T> filtered, DttRequestForm form) where T : ModelBase {

            if (form.order?.Count > 0) {

                foreach (var item in form.order) {
                    
                    item.colname = form.columns[item.column].data;

                    if (item.colname == "Log") {
                        filtered = filtered
                            .OrderBy("CreatedDate " + item.dir);
                        if (item.dir == "asc")
                            filtered = filtered.OrderBy(t => t.UpdatedDate ?? t.CreatedDate);
                        else
                            filtered = filtered.OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
                    } else {

                        filtered = filtered.OrderBy(item.colname + " " + item.dir);
                    }
                }

            } else {

                filtered = filtered.OrderByDescending(t => t.CreatedDate).OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            }

            if (form.length.HasValue) {
                return filtered.Skip((form.start / form.length.Value) * form.length.Value)
                .Take(form.length.Value);
            } else {
                return filtered;
            }
        }

        public IQueryable<T> GetDynamicFilteredData<T>(IQueryable<T> rawData, DttRequestForm form) where T : ModelBase {

            StringBuilder query = new StringBuilder("");

            if ((form.dyamicSearch?.Count ?? 0) > 0) {

                var objParams = new List<object>();

                var filters = form.dyamicSearch.OrderBy(t => t.index)
                    .Where(t => !string.IsNullOrEmpty(t.data) 
                    && !string.IsNullOrEmpty(t.filter)
                    && t.type != "custom")
                    .ToList();

                foreach (var filter in filters) {

                    if (filter.value == null && filter.type != "string") {
                        continue;
                    } else if(filter.value == null && filter.type == "string") {
                        filter.value = "";
                    }
                    
                    if (string.IsNullOrEmpty(query.ToString())) {
                        filter.logicgate = null;
                    }


                    query.Append(" ");
                    query.Append(filter.logicgate ?? "");
                    query.Append(" ");



                    var template = "";

                    if(filter.data == "#Status") {
                        if (filter.type == "status") {

                            query.Append(FilterStatus(filter)+" ");

                        } else {
                            continue;
                        }
                    } else {
                        if (filter.type == "string") {

                            filter.value = filter.value.ToLower();

                            if (filter.filter == "contains") {
                                template = "{0}.ToLower().Contains(@{1})";
                            } else if (filter.filter == "in") {
                                template = "@{1}.Contains({0}.ToLower())";
                            } else {
                                template = "{0}.ToLower() {2} @{1}.ToLower()";
                            }

                        } else if (filter.type == "nullabledatetime") {
                            template = "{0}.Value.Date {2} @{1}";
                        } else if (filter.type == "date") {
                            template = "{0}.Date {2} @{1}";
                        } else {
                            template = "{0} {2} @{1}";
                        }

                    //} else if (filter.type == "nullabledatetime") {
                    //        template = "{0}.Value.Date {2} @{1}";
                    //    } else if (filter.type == "date") {
                    //        template = "{0}.Date {2} @{1}";
                    //    } else {
                    //        template = "{0} {2} @{1}";
                    //    }

                        query.AppendFormat(template, filter.data, objParams.Count, filter.filter);

                        if (filter.type == "string")
                            objParams.Add(filter.value);
                        else if (filter.type == "number")
                            objParams.Add(filter.value.SafeDecimalConvert());
                        else if (filter.type == "date")
                            objParams.Add(filter.value.SafeDateConvert()?.Date);
                        else if (filter.type == "nullabledatetime")
                            objParams.Add(filter.value.SafeDateConvert()?.Date);
                        else if (filter.type == "datetime")
                            objParams.Add(filter.value.SafeDateConvert());
                        else if (filter.type.ToLower() == "bool")
                            objParams.Add(filter.value.SafeConvert<bool>());

                    }
                    
                    
                }


                rawData = rawData.Where(query.ToString(), objParams.ToArray()).BindDbFunctions();

                try {
                    rawData = ProcessCustomSearch(rawData, form.dyamicSearch.OrderBy(t => t.index)
                    .Where(t => !string.IsNullOrEmpty(t.data)
                    && !string.IsNullOrEmpty(t.filter)
                    && t.type == "custom")
                    .ToList());
                } catch (Exception exc) {
                    
                }
            }
            return rawData;
        }

        public virtual IQueryable<T> ProcessCustomSearch<T>(IQueryable<T> baseFiltered,List<DttDynamicSearch> searches) where T : ModelBase {
            
            return baseFiltered;
        }

        public DttResponseForm<TForm> ProcessDatatableResult<TForm, TModel>(IQueryable<TModel> query, DttRequestForm form) 
            where TForm : FormModelBase, new()
            where TModel : ModelBase
        {
            var result = new DttResponseForm<TForm>();

            var user = GetCurrentUser();

            if (user != null) {

                try {
                    var roles = HttpContext.Current.Cache["roles"] as IEnumerable<ApplicationRole>;

                    if(roles == null) {
                        roles = context.Roles.Include("Actions").Where(t => t.IsActive == true && t.IsDeleted != true && t.IsDraft == false).ToList();
                        HttpContext.Current.Cache["roles"] = roles;
                    }

                    var actions = roles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList().SelectMany(t => t.Actions).ToList();

                    if(!actions.Any(t => t.ActionName == AppActions.Restore_Delete)) {
                        query = query.Where(t => t.IsDeleted != true);
                    }

                } catch (Exception exc) {
                    ErrorLog(exc);
                    query = query.Where(t => t.IsDeleted != true);
                }

            } else {
                query = query.Where(t => t.IsDeleted != true);
            }

            result.recordsTotal = query.Count();

            query = GetDynamicFilteredData(query, form);

            result.recordsFiltered = query.Count();

            result.data = Mapper.Map<List<TModel>,List<TForm>>(GetPaging(query,form).ToList());
            result.draw = form.draw;
            
            return result;
        }

        public virtual void RestoreDelete(string id) {

        }

        public void RestoreDeleteData<T>(string id) where T : ModelBase {
            var result = context.Set<T>().Where(t => t.Id == id).FirstOrDefault();

            if (result != null) {

                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                result.IsDeleted = false;
                result.UpdatedDate = DateTime.Now;
                result.UpdatedBy = user?.UserName ?? "";
            }
            context.SaveChanges();
        }

        public string FilterStatus(DttDynamicSearch form) {
            var result = "";

            if (form.value == "Saved") {
                result = $"IsDraft == false";
            } else if(form.value == "Draft") {
                result = $"IsDraft == true";
            } else if(form.value == "Deleted") {
                result = $"IsDeleted == true";
            }
            
            return result;
        }

        public virtual bool IsSameDataExists<TModel,TForm>(TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new() {


            if (obj.Id != null) return false;

            var query = context.Set<TModel>().Where(t => t.IsDeleted != true && t.IsDraft == false).AsQueryable();

            return IsSameDataExists(query, obj);

        }

        public virtual bool IsSameDataExists<TModel, TForm>(IQueryable<TModel> rawData, TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new() {

            if (obj.Id != null) return false;

            rawData = rawData.Where(t => t.IsDeleted != true && t.IsDraft == false);

            var type = typeof(TModel);
            var formtype = typeof(TForm);
            var props = formtype.GetProperties().Where(t => t.DeclaringType == formtype);

            var parameter = Expression.Parameter(type, "x");
            Expression and = null;
            foreach (var prop in props) {
                var modelprop = type.GetProperty(prop.Name);
                if (modelprop == null) continue;

                var attrCompare = prop.GetCustomAttribute<CompareDataAttribute>();

                if (attrCompare?.Excluded == true) continue;

                Expression equal = null;
                if (prop.PropertyType.IsBasicType()) {

                    if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(DateTime?)) {
                        //equal = Expression.Call(Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)), "Equals", Type.EmptyTypes, Expression.Convert(Expression.Constant(prop.GetValue(obj, null)), typeof(Object)));
                        equal = Expression.Equal(
                               Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)),
                               Expression.Convert(Expression.Constant(prop.GetValue(obj, null)),prop.PropertyType)
                           );
                    } else {

                        equal = Expression.Equal(
                               Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)),
                               Expression.Constant(prop.GetValue(obj, null))
                           );
                    }


                } else {

                    if (prop.PropertyType.IsSubclassOf(typeof(ModelBase))) {
                        var idAtt = prop.GetCustomAttribute<IdFieldAttribute>();

                        if (idAtt != null) {

                            equal = Expression.Equal(
                                   Expression.MakeMemberAccess(
                                       Expression.MakeMemberAccess(parameter, type.GetMember(prop.Name).FirstOrDefault()),
                                       type.GetProperty(prop.Name).PropertyType.GetProperty("Id")),
                                   Expression.Constant(formtype.GetProperty(idAtt.FieldName).GetValue(obj, null))
                               );

                        }
                    }

                }
                if (equal != null) {
                    if (and == null) {
                        and = equal;
                    } else {
                        and = Expression.And(and, equal);
                    }
                }

            }



            return rawData.Any(Expression.Lambda<Func<TModel, bool>>(and, parameter));

        }

        public IQueryable<TModel> GetCheckExistQuery<TModel, TForm>(TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new() {

            var rawData = context.Set<TModel>().AsQueryable();

            if (obj.Id != null) return null;

            rawData = rawData.Where(t => t.IsDeleted != true && t.IsDraft == false);

            var type = typeof(TModel);
            var formtype = typeof(TForm);
            var props = formtype.GetProperties().Where(t => t.DeclaringType == formtype);

            var parameter = Expression.Parameter(type, "x");
            Expression and = null;
            foreach (var prop in props) {
                var modelprop = type.GetProperty(prop.Name);
                if (modelprop == null) continue;


                var compAtt = prop.GetCustomAttribute<CompareDataAttribute>();

                if (compAtt?.Excluded == true) continue;

                Expression equal = null;
                if (prop.PropertyType.IsBasicType()) {

                    if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(DateTime?)) {
                        //equal = Expression.Call(Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)), "Equals", Type.EmptyTypes, Expression.Convert(Expression.Constant(prop.GetValue(obj, null)), typeof(Object)));
                        equal = Expression.Equal(
                               Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)),
                               Expression.Convert(Expression.Constant(prop.GetValue(obj, null)), prop.PropertyType)
                           );
                    } else {

                        equal = Expression.Equal(
                               Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)),
                               Expression.Constant(prop.GetValue(obj, null))
                           );
                    }


                } else {

                    if (prop.PropertyType.IsSubclassOf(typeof(ModelBase))) {
                        var idAtt = prop.GetCustomAttribute<IdFieldAttribute>();

                        if (idAtt != null) {

                            equal = Expression.Equal(
                                   Expression.MakeMemberAccess(
                                       Expression.MakeMemberAccess(parameter, type.GetMember(prop.Name).FirstOrDefault()),
                                       type.GetProperty(prop.Name).PropertyType.GetProperty("Id")),
                                   Expression.Constant(formtype.GetProperty(idAtt.FieldName).GetValue(obj, null))
                               );

                        }
                    }

                }
                if (equal != null) {
                    if (and == null) {
                        and = equal;
                    } else {
                        and = Expression.And(and, equal);
                    }
                }

            }



            return rawData.Where(Expression.Lambda<Func<TModel, bool>>(and, parameter));

        }
        
        public FileModel WriteFile(FileModel file) {
            try {

                var controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                var areaname = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]?.ToString();

                var dest = ConfigurationManager.AppSettings["docstorage"].ToString();

                if (dest.StartsWith("~")) {
                    dest = HttpContext.Current.Server.MapPath(dest);
                }

                var dailyDoc = dest + "\\" + DAILY_FOLDER_NAME + "\\" + DateTime.Now.Date.ToString("yyyyMMdd") + "\\" + Path.GetFileName(file.FileName);

                var finaldest = "";// dest + areaname + "\\" + controllerName + "\\" + Path.GetFileName(file.FileName);

                if (string.IsNullOrEmpty(areaname)) {
                    finaldest = dest + controllerName + "\\" + Path.GetFileName(file.FileName);
                } else {
                    finaldest = dest + areaname + "\\" + controllerName + "\\" + Path.GetFileName(file.FileName);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(finaldest));
                try {

                    if (File.Exists(finaldest)) File.Delete(finaldest);

                    File.WriteAllBytes(finaldest, file.Bytes);
                    file.FileName = finaldest;

                } catch (Exception exc) {
                    
                }

                try {
                    Directory.CreateDirectory(Path.GetDirectoryName(dailyDoc));
                    if (File.Exists(dailyDoc)) File.Delete(dailyDoc);

                    File.WriteAllBytes(dailyDoc, file.Bytes);

                } catch (Exception exc) {

                }

            } catch (Exception exc) {
                
            }


            return file;

        }

        public ApplicationUser GetUserByEmail(string logname) {

            logname = logname.Split('@')[0] + "@";

            return context.Users.Where(t => t.Email.Contains(logname)).FirstOrDefault();
        }

        public ApplicationUser GetCurrentUser() {
            return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        protected void ErrorLog(Exception e) {
            try {
                ErrorSignal.FromCurrentContext().Raise(e);
            } catch (Exception exc) {
                
            }
            
        }
    }
}