using App.DataAccess.Identity;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IAccountServices : IServiceBase {
        DttResponseForm<ApplicationUser> GetList(DttRequestWithDate form);
        DttResponseForm<ApplicationRole> GetRoleList(DttRequestWithDate form);
        void DeleteUser(string id, string userName);
        void DeleteRole(string id, string userName);
        RoleViewModel InsertRole(RoleViewModel model);
        RoleViewModel GetRoleViewModel(string id);
        IEnumerable<ActionAuthorization> GetActionList();
        List<ApplicationUser> GetAllActiveUser();
    }
}
