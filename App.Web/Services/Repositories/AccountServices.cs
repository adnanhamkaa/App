using App.DataAccess;
using App.DataAccess.Identity;
using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using App.Web.Utilities;
using System.Text;
using AutoMapper;

namespace App.Web.Services.Repositories {
    public class AccountServices : ServiceBase, IAccountServices {
        private IMasterDataServices _setupSvc;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AccountServices(ApplicationDbContext context, IMasterDataServices setupSvc, ApplicationUserManager userManager, ApplicationRoleManager roleManager) {
            this.context = context;
            this._setupSvc = setupSvc;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        
        public List<ApplicationUser> GetAllActiveUser() {
            return _userManager.Users.Where(t => t.IsActive == true).ToList();
        }

        public DttResponseForm<ApplicationUser> GetList(DttRequestWithDate form) {

            var query = _userManager.Users;

            var result = new DttResponseForm<ApplicationUser>();

            var user = GetCurrentUser();

            if (user != null) {
                var isAdmin = user.Roles.FirstOrDefault(t => ROLEADMINIDS.Contains(t.RoleId));
                if (isAdmin == null) {
                    query = query.Where(t => t.IsDeleted != true);
                }
            } else {
                query = query.Where(t => t.IsDeleted != true);
            }

            result.recordsTotal = query.Count();

            query = GetUserDynamicFilteredData(query, form);

            result.recordsFiltered = query.Count();

            result.data = GetUserPaging(query, form).ToList();
            result.draw = form.draw;

            return result;

            //var result = new DttResponseForm<ApplicationUser>();
            //result.draw = form.draw;
            //form.search.value = (form.search.value ?? "").ToLower();
            ////form.length = int.MaxValue;

            //var query = _userManager.Users;


            //if (form.order?.Count > 0) {

            //    foreach (var item in form.order) {


            //        item.colname = form.columns[item.column].data;

            //        if (item.colname == "Log") {
            //            query = query
            //                .OrderBy("CreatedDate " + item.dir);
            //            if (item.dir == "asc")
            //                query = query.OrderBy(t => t.UpdatedDate ?? t.CreatedDate);
            //            else
            //                query = query.OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            //        } else {

            //            query = query.OrderBy(item.colname + " " + item.dir);
            //        }
            //    }

            //} else {

            //    query = query.OrderByDescending(t => t.CreatedDate).OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            //}

            //result.data = form.length.HasValue ? query.Skip(form.start * form.length.Value).Take(form.length.Value).ToList() : query.ToList();

            //result.recordsTotal = query.Count();
            //result.recordsFiltered = result.recordsTotal;

            //return result;
        }

        public DttResponseForm<ApplicationRole> GetRoleList(DttRequestWithDate form) {

            var query = _roleManager.Roles.Include("Actions");

            var result = new DttResponseForm<ApplicationRole>();

            var user = GetCurrentUser();

            if (user != null) {
                var isAdmin = user.Roles.FirstOrDefault(t => ROLEADMINIDS.Contains(t.RoleId));
                if (isAdmin == null) {
                    query = query.Where(t => t.IsDeleted != true);
                }
            } else {
                query = query.Where(t => t.IsDeleted != true);
            }

            result.recordsTotal = query.Count();

            query = GetRoleDynamicFilteredData(query, form);

            result.recordsFiltered = query.Count();

            result.data = GetRolePaging(query, form).ToList();
            result.draw = form.draw;

            return result;

            //var result = new DttResponseForm<ApplicationRole>();
            //result.draw = form.draw;
            //form.search.value = (form.search.value ?? "").ToLower();
            ////form.length = int.MaxValue;

            //var query = _roleManager.Roles.Include("Actions");


            //if (form.order?.Count > 0) {

            //    foreach (var item in form.order) {


            //        item.colname = form.columns[item.column].data;

            //        if (item.colname == "Log") {
            //            query = query
            //                .OrderBy("CreatedDate " + item.dir);
            //            if (item.dir == "asc")
            //                query = query.OrderBy(t => t.UpdatedDate ?? t.CreatedDate);
            //            else
            //                query = query.OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            //        } else {

            //            query = query.OrderBy(item.colname + " " + item.dir);
            //        }
            //    }

            //} else {

            //    query = query.OrderByDescending(t => t.CreatedDate).OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            //}

            //result.data = form.length.HasValue ? query.Skip(form.start * form.length.Value).Take(form.length.Value).ToList() : query.ToList();

            //result.recordsTotal = query.Count();
            //result.recordsFiltered = result.recordsTotal;

            //return result;
        }

        public ApplicationUser GetUserByEmail(string logname) {

            logname = logname.Split('@')[0] + "@";

            return context.Users.Where(t => t.Email.Contains(logname)).FirstOrDefault();
        }

        public void DeleteUser(string id, string userName) {
            var user = _userManager.FindById(id);

            if(user != null) {
                _userManager.Delete(user);
            }

        }
        
        public void DeleteRole(string id, string userName) {
            var role = _roleManager.FindById(id);

            if (role != null) {
                _roleManager.Delete(role);
            }

            try {
                var roles = context.Roles.Include("Actions").Where(t => t.IsActive == true && t.IsDeleted != true && t.IsDraft == false).ToList();
                HttpContext.Current.Cache["roles"] = roles;
            } catch (Exception exc) {
                ErrorLog(exc);
            }
        }
        
        public RoleViewModel InsertRole(RoleViewModel model) {

            var current = context.Roles.Include("Actions").Where(t => t.Id == model.Id).FirstOrDefault();

            if(current == null) {
                current = new ApplicationRole();
                current.Init();
                current.Name = model.Name;

                current.Actions = context.ActionAuthorization.ToList().Where(t => model.SelectedActions.Any(a => a == t.Id)).ToList();

                context.Roles.Add(current);
                

            } else {
                current.Name = model.Name;
                var todelete = current.Actions?.ToList();
                todelete.ForEach(t => current.Actions.Remove(t));

                current.Actions = context.ActionAuthorization.ToList().Where(t => model.SelectedActions.Any(a => a == t.Id)).ToList();
                
            }

            context.SaveChanges();
            model.Id = current.Id;
            model.Actions = current.Actions;

            try {
                var roles = context.Roles.Include("Actions").Where(t => t.IsActive == true && t.IsDeleted != true && t.IsDraft == false).ToList();
                HttpContext.Current.Cache["roles"] = roles;
            } catch (Exception exc) {
                ErrorLog(exc);
            }

            return model;

        }

        public RoleViewModel GetRoleViewModel(string id) {
            var role = context.Roles.Include("Actions").Where(t => t.Id == id).FirstOrDefault();

            var result = new RoleViewModel();
            result.Actions = new List<ActionAuthorization>();
            result.ActionsOpt = new List<ActionAuthorization>();
            result.SelectedActions = new List<string>();

            if (role != null) {
                result.Id = role.Id;
                result.Name = role.Name;
                result.Actions = role.Actions;
            }

            return result;

        }

        public IEnumerable<ActionAuthorization> GetActionList() {
            return context.ActionAuthorization.Where(t => t.IsDeleted != true).ToList();
        }

        public IQueryable<ApplicationUser> GetUserDynamicFilteredData(IQueryable<ApplicationUser> rawData, DttRequestForm form) {

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
                    } else if (filter.value == null && filter.type == "string") {
                        filter.value = "";
                    }

                    if (string.IsNullOrEmpty(query.ToString())) {
                        filter.logicgate = null;
                    }


                    query.Append(" ");
                    query.Append(filter.logicgate ?? "");
                    query.Append(" ");



                    var template = "";

                    if (filter.data == "#Status") {
                        if (filter.type == "status") {

                            query.Append(FilterStatus(filter) + " ");

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

                //try {
                //    //rawData = ProcessCustomSearch(rawData, form.dyamicSearch.OrderBy(t => t.index)
                //    //.Where(t => !string.IsNullOrEmpty(t.data)
                //    //&& !string.IsNullOrEmpty(t.filter)
                //    //&& t.type == "custom")
                //    //.ToList());
                //} catch (Exception exc) {

                //}
            }
            return rawData;
        }

        public IQueryable<ApplicationUser> GetUserPaging(IQueryable<ApplicationUser> filtered, DttRequestForm form) {

            if (form.order?.Count > 0) {

                foreach (var item in form.order) {


                    item.colname = form.columns[item.column].data;

                    if (item.colname != "Log") {

                        filtered = filtered.OrderBy(item.colname + " " + item.dir);
                    }
                }

            } else {

                filtered = filtered.OrderByDescending(t => t.UpdatedDate?? t.CreatedDate);
            }

            if (form.length.HasValue) {
                return filtered.Skip((form.start / form.length.Value) * form.length.Value)
                .Take(form.length.Value);
            } else {
                return filtered;
            }
        }

        public IQueryable<ApplicationRole> GetRoleDynamicFilteredData(IQueryable<ApplicationRole> rawData, DttRequestForm form) {

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
                    } else if (filter.value == null && filter.type == "string") {
                        filter.value = "";
                    }

                    if (string.IsNullOrEmpty(query.ToString())) {
                        filter.logicgate = null;
                    }


                    query.Append(" ");
                    query.Append(filter.logicgate ?? "");
                    query.Append(" ");



                    var template = "";

                    if (filter.data == "#Status") {
                        if (filter.type == "status") {

                            query.Append(FilterStatus(filter) + " ");

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

                //try {
                //    //rawData = ProcessCustomSearch(rawData, form.dyamicSearch.OrderBy(t => t.index)
                //    //.Where(t => !string.IsNullOrEmpty(t.data)
                //    //&& !string.IsNullOrEmpty(t.filter)
                //    //&& t.type == "custom")
                //    //.ToList());
                //} catch (Exception exc) {

                //}
            }
            return rawData;
        }

        public IQueryable<ApplicationRole> GetRolePaging(IQueryable<ApplicationRole> filtered, DttRequestForm form) {

            if (form.order?.Count > 0) {

                foreach (var item in form.order) {


                    item.colname = form.columns[item.column].data;

                    if (item.colname != "Log") {

                        filtered = filtered.OrderBy(item.colname + " " + item.dir);
                    }
                }

            } else {

                filtered = filtered.OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate);
            }

            if (form.length.HasValue) {
                return filtered.Skip((form.start / form.length.Value) * form.length.Value)
                .Take(form.length.Value);
            } else {
                return filtered;
            }
        }


    }
}