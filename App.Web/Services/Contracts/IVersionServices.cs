using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Services.Contracts {
    public interface IVersionServices : IServiceBase {
        string VersionCheck();
    }
}
