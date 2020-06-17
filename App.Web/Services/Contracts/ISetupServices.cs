using App.DataAccess;
using App.Web.Models;
using System.Collections.Generic;

namespace App.Web.Services.Contracts
{
    public interface IMasterDataServices {
        //IEnumerable<T> GetAll<T>() where T : ModelBase;
        IEnumerable<T> GetAll<T>(bool includeDraft = false) where T : ModelBase;
        IEnumerable<TForm> GetAll<TModel,TForm>(bool includeDraft = false) where TModel : ModelBase;
        IEnumerable<TForm> GetAll<TModel, TForm>(bool includeDraft = false, params string[] includes) where TModel : ModelBase;
        TModel Upsert<TModel, TForm>(TForm form,params string[] includes) where TModel : ModelBase, new() where TForm : FormModelBase, new();
        TModel Delete<TModel>(string id) where TModel : ModelBase;
        T GetById<T>(string id, params string[] includes) where T : ModelBase;
        //IEnumerable<T> GetAll<T>(params string[] includes) where T : ModelBase; 
        IEnumerable<T> GetAll<T>(bool includeDraft = false, params string[] includes) where T : ModelBase;
    }
}
