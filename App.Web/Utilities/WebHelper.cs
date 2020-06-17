using App.DataAccess;
using App.Utilities;
using App.DataAccess.Identity;
using App.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using App.Web.Utilities;

namespace App.Web
{
    public class WebHelper
    {
        static string[] ROLEADMINIDS => ConfigurationManager.AppSettings["RoleAdminId"].ToString().Split(',');
        public static ApplicationUser GetUser() {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        public static List<string> GetUserActions() {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            List<ApplicationRole> roles;

            if (HttpContext.Current.Cache["roles"] != null && false) {
                roles = (List<ApplicationRole>)HttpContext.Current.Cache["roles"];
            } else {
                var context = new ApplicationDbContext();
                roles = context.Roles.Include("Actions").ToList();
                HttpContext.Current.Cache["roles"] = roles;
            }

            if (user == null) {
                return null;
            }

            return roles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList().SelectMany(t => t.Actions).Select(t => t.ActionName).Distinct().ToList();
        }

        public static bool IsUserAdmin() {
            var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            try {
                if (user != null) {
                    var isAdmin = user.Roles.FirstOrDefault(t => ROLEADMINIDS.Contains(t.RoleId));
                    if (isAdmin != null) {
                        return true;
                    }
                }
            } catch (Exception exc) {

            }


            return false;
        }
        public static ApplicationUser GetUserByEmail(string email) {
            var context = new ApplicationDbContext();
            return context.Users.Where(t => t.Email.StartsWith(email + "@") || t.Email == email).FirstOrDefault();
            //return context.Users.Where(t => t.Email == email).FirstOrDefault();
        }

        public static ApplicationUser GetUserByEmailOrUserName(string key) {
            var context = new ApplicationDbContext();
            return context.Users.Where(t => t.Email.ToLower().Equals(key.ToLower()) || t.UserName.ToLower().Equals(key.ToLower())).FirstOrDefault();
            //return context.Users.Where(t => t.Email == email).FirstOrDefault();
        }



    }

    public static class WebHelperExtension
    {
        public static void SetCreated(this FormModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.CreatedBy = user?.UserName;//?.Split('@')[0];
            } catch (Exception exc) {

            }

            model.CreatedDate = DateTime.Now;
        }

        public static void SetUpdated(this FormModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.UpdatedBy = user?.UserName;
            } catch (Exception exc) {

            }

            model.UpdatedDate = DateTime.Now;
        }

        public static void SetCreated(this ModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.CreatedBy = user?.UserName;
            } catch (Exception exc) {

            }

            model.CreatedDate = DateTime.Now;
        }

        public static void SetUpdated(this ModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.UpdatedBy = user?.UserName;
            } catch (Exception exc) {

            }

            model.UpdatedDate = DateTime.Now;
        }


        public static void SetCreated(this IModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.CreatedBy = user?.UserName;
            } catch (Exception exc) {

            }

            model.CreatedDate = DateTime.Now;
        }

        public static void SetUpdated(this IModelBase model) {
            try {
                model.CreatedBy = "SYSTEM";
                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.UpdatedBy = user?.UserName;
            } catch (Exception exc) {

            }

            model.UpdatedDate = DateTime.Now;
        }

        public static bool IsSameDataExists<TModel, TForm>(IQueryable<TModel> rawData, TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new() {

            if (obj.Id != null) return false;

            rawData = rawData.Where(t => t.IsDeleted != true && t.IsDraft == false);

            var type = typeof(TModel);
            var formtype = typeof(TForm);
            var props = formtype.GetProperties().Where(t => t.DeclaringType == formtype);

            var parameter = Expression.Parameter(type, "x");
            BinaryExpression and = null;
            foreach (var prop in props) {
                var modelprop = type.GetProperty(prop.Name);
                if (modelprop == null) continue;

                BinaryExpression equal = null;
                if (prop.PropertyType.IsBasicType()) {
                    equal = Expression.Equal(
                           Expression.MakeMemberAccess(parameter, type.GetProperty(prop.Name)),
                           Expression.Constant(prop.GetValue(obj, null))
                       );

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

        public static IQueryable<T> GetActiveData<T>(this IQueryable<T> model) where T : ModelBase {

            return model.Where(t => t.IsDeleted != true && t.IsDraft == false);

        }
        
    }
}