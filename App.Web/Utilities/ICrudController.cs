using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Web.Utilities
{
    public interface ICrudController<TVm> where TVm : ViewModelBase
    {
        [HttpGet]
        ActionResult Entry(string id, string opt);

        [HttpPost]
        ActionResult Entry(TVm vm);

        [HttpGet]
        ActionResult Index();

        [HttpPost]
        ActionResult GetList(DttRequestForm form);

        [HttpPost]
        ActionResult Delete(string id);
    }
}
