using App.DataAccess;
using App.DataAccess.Identity;
using App.Utilities.Model;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IServiceBase {
        ApplicationDbContext context { get; set; }
        
        void Commit();

        DbContextTransaction BeginTran();

        bool IsSameDataExists<TModel, TForm>(IQueryable<TModel> rawData, TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new();

        bool IsSameDataExists<TModel, TForm>(TForm obj)
            where TModel : ModelBase, new()
            where TForm : FormModelBase, new();

        ApplicationUser GetCurrentUser();
        void RestoreDelete(string id);
        FileModel WriteFile(FileModel file);
        IQueryable<T> ProcessCustomSearch<T>(IQueryable<T> baseFiltered, List<DttDynamicSearch> searches) where T : ModelBase;
    }
}
