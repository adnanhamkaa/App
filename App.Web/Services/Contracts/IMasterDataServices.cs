using App.DataAccess;
using App.Utilities.Model;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {

    public interface ICrudServices<TModel,TForm> : IServiceBase
        where TModel : ModelBase,new() where TForm : FormModelBase, new() 
         {

        List<TForm> GetAll();
        List<TForm> GetAllWithDetail();
        TForm GetById(string id);
        TForm Insert(TForm form, bool commit = true);
        TModel InsertWithoutCommit(TForm form);
        TForm Delete(string id, bool commit = true);
        DttResponseForm<TForm> GetList<T>(T form) where T : DttRequestWithDate;
        FileModel GetDoc(string id);
        bool CheckIsSameDataExists(TForm form);
        DttResponseForm<TForm> GetDynamicList(DttRequestForm form);
        TForm DeleteData(string id, string userName);
    }
}
