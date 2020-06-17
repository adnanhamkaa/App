using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Services.Repositories {
    public class VersionServices : ServiceBase, IVersionServices {
        public string VersionCheck() {
            //return "v0.0.1";
			return App.Web.Utilities.CommonHelper.GetVersion();        }
    }
}