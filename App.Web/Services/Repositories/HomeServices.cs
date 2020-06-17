using App.Utilities.Model;
using App.Web.Models;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Services.Repositories {
    public class HomeServices : ServiceBase, IHomeServices {
        private IFileManServices _filemanSvc;


        public HomeServices(IFileManServices filemanSvc) {
            _filemanSvc = filemanSvc;
        }


        public FileModel GetDoc(string name) {

            return _filemanSvc.LoadTemplate("UserManual\\"+name);

        }
    }
}