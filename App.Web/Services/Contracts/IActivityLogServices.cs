using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IActivityLogServices : IServiceBase {
        DttResponseForm<ActivityLogModel> GetList(DttRequestWithDate form);
    }
}
