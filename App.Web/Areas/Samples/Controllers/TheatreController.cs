using App.DataAccess;
using App.Web.Areas.Samples.Model;
using App.Web.Services.Contracts;
using App.Web.Utilities;

namespace App.Web.Areas.Samples.Controllers
{
    [AppAuth]
    [Module(Name = AppModule.Samples_MasterData)]
    public class TheatreController : BaseCrudController<ITheatreServices, TheatreViewModel, Theatre, TheatreForm>
    {
        
    }
}