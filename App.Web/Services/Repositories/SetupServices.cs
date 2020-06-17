using App.DataAccess;
using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using App.Web.Utilities;

namespace App.Web.Services.Repositories {
    public class SetupServices : ServiceBase, IMasterDataServices {

        public SetupServices(ApplicationDbContext context) {
            this.context = context;
        }

        //public IEnumerable<T> GetAll<T>() where T : ModelBase {
        //    return context.Set<T>().Where(t => t.IsDeleted != true).ToList();
        //}

        //public IEnumerable<T> GetActiveData<T>() where T : ModelBase {
        //    return context.Set<T>().Where(t => t.IsDeleted != true).ToList();
        //}


        public IEnumerable<T> GetAll<T>(bool includeDraft = false) where T : ModelBase {
            return context.Set<T>().Where(t => t.IsDeleted != true && (includeDraft || t.IsDraft == false)).ToList();
        }

        public IEnumerable<TForm> GetAll<TModel,TForm>(bool includeDraft = false) where TModel : ModelBase {
            return AutoMapper.Mapper.Map<IEnumerable<TForm>>(context.Set<TModel>().Where(t => t.IsDeleted != true && (includeDraft || t.IsDraft == false)).ToList());
        }

        public IEnumerable<TForm> GetAll<TModel, TForm>(bool includeDraft = false, params string[] includes) where TModel : ModelBase {
            return AutoMapper.Mapper.Map<IEnumerable<TForm>>(context.Set<TModel>().Where(t => t.IsDeleted != true && (includeDraft || t.IsDraft == false)).IncludeAll(includes).ToList());
        }

        public IEnumerable<T> GetAll<T>(params string[] includes) where T : ModelBase {
            return context.Set<T>().Where(t => t.IsDeleted != true).IncludeAll(includes).ToList();
        }
        
        public IEnumerable<T> GetAll<T>(bool includeDraft, params string[] includes) where T : ModelBase {
            return context.Set<T>().Where(t => t.IsDeleted != true && (includeDraft || t.IsDraft == false)).IncludeAll(includes).ToList();
        }

        public T GetById<T>(string id,params string[] includes) where T : ModelBase {
            return context.Set<T>().Where(t => t.IsDeleted != true && t.Id == id).IncludeAll(includes).FirstOrDefault();
        }

        public TModel Upsert<TModel, TForm>(TForm form,params string[] includes) where TModel : ModelBase, new() where TForm : FormModelBase, new() {

            var result = context.Set<TModel>().IncludeAll(includes).Where(t => t.Id == form.Id).FirstOrDefault();

            if(result == null) {
                form.SetCreated();
                result = new TModel();
                result = Mapper.Map(form,result);
                result.Init();
                context.Set<TModel>().Add(result);
            } else {
                form.SetUpdated();
                result = Mapper.Map(form, result);
            }
            
            return result;
        }

        public TModel Delete<TModel>(string id) where TModel : ModelBase {

            var result = context.Set<TModel>().Where(t => t.Id == id).FirstOrDefault();

            if (result != null) {

                var user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                
                result.IsDeleted = true;
                result.UpdatedDate = DateTime.Now;
                result.UpdatedBy = user?.UserName??"";
            }

            return result;
        }
        

    }
}