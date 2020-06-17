using App.DataAccess;
using App.Web.Services.Contracts;
using App.Web.Services.Repositories;
using Hangfire;
using Ninject.Modules;
using Ninject.Web.Common;
using System.Linq;

namespace App.Web.Services
{
    public class ApplicationNinjectModule : NinjectModule {
        /// <inheritdoc />
        public override void Load() {

            Kernel.Bind<IActivityLogServices>().To<ActivityLogServices>().InAppScope();
            Kernel.Bind<IMasterDataServices>().To<SetupServices>().InAppScope();
            Kernel.Bind<ApplicationDbContext>().ToSelf().InAppScope();
            Kernel.Bind<IWordTextReplacementServices>().To<WordTextReplacementServices>().InAppScope();
            Kernel.Bind<IWorkflowServices>().To<WorkflowServices>().InAppScope();
            Kernel.Bind<IAccountServices>().To<AccountServices>().InAppScope();
            Kernel.Bind<IFileManServices>().To<FileManServices>().InAppScope();
            Kernel.Bind<IPdfServices>().To<PdfServices>().InAppScope();
            Kernel.Bind<IKeyValueStoreServices>().To<KeyValueStoreServices>().InAppScope();
            Kernel.Bind<ISysLogger>().To<SysLogger>().InAppScope();
            Kernel.Bind<IEmailServices>().To<EmailServices>().InAppScope();
            Kernel.Bind<IExcelFileServices>().To<ExcelFileServices>().InAppScope();
            Kernel.Bind<IMigrasiDataServices>().To<MigrasiDataServices>().InAppScope();
            Kernel.Bind<IVersionServices>().To<VersionServices>().InAppScope();
            Kernel.Bind<IAppReminderJobServices>().To<AppReminderJobServices>().InAppScope();
            Kernel.Bind<IHomeServices>().To<HomeServices>().InAppScope();
            
            
            #region Sample
            Kernel.Bind<IMovieServices>().To<MovieServices>().InAppScope();
            Kernel.Bind<ITheatreServices>().To<TheatreServices>().InAppScope();
            Kernel.Bind<IShowTimeServices>().To<ShowTimeServices>().InAppScope();
            Kernel.Bind<ITicketServices>().To<TicketServices>().InAppScope();
            #endregion Sample

        }

    }

    public static class NinjectBindHelper {
        public static void InAppScope<T>(this Ninject.Syntax.IBindingInSyntax<T> binding) {
            binding.InNamedOrBackgroundJobScope(context => context.Kernel.Components.GetAll<INinjectHttpApplicationPlugin>().Select(c => c.GetRequestScope(context)).FirstOrDefault(s => s != null));
        }
    }
}