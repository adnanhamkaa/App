using App.DataAccess;
using App.Web.Services.Contracts;
using System;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class DbMigrationsController : Controller
    {
        private static readonly object _migrationLock = new object();
        private ISysLogger _logger;
        private IVersionServices _versionServices;

        public DbMigrationsController(ISysLogger logger,
            IVersionServices versionServices) {
            _logger = logger;
            _versionServices = versionServices;
        }

        [HttpGet]
        public ActionResult Update(string password, string target = null) {
            if (password != WebAppSettings.DbMigrationSecret) {
                return HttpNotFound();
            }

            try {
                lock (_migrationLock) {
                    var migrator = new DatabaseMigrator<ApplicationDbContext, App.DataAccess.Migrations.Configuration>();
                    migrator.MigrateToLatestVersion(true, string.IsNullOrWhiteSpace(target) ? null : target);
                    return Json(new {
                        Status = true,
                        Message =
                            $"Success in migrating DB. Target migration: {(string.IsNullOrWhiteSpace(target) ? "#LATEST#" : target)}"
                    }, JsonRequestBehavior.AllowGet);
                }
            } catch (Exception ex) {
                _logger.LogError(ex);
                return Json(new {
                    Status = false,
                    Error = ex.Message,
                    ErrorInner = ex.InnerException?.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult UpdateInitData(string password, string target = null) {
            if (password != WebAppSettings.DbMigrationSecret) {
                return HttpNotFound();
            }

            var context = new ApplicationDbContext();

            context.SaveChanges();

            return Json(new {
                Status = true,
                Message = $"Success in init data DB"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
