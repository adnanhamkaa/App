using System.Data.Entity.Migrations;
using System.Data.Entity;
namespace App.DataAccess
{
    public class DatabaseMigrator<TDbContext, TMigrationConfig>
        where TDbContext : DbContext
        where TMigrationConfig : DbMigrationsConfiguration<TDbContext>, new()
    {
        /// <summary>
        ///     Do the DB migration
        /// </summary>
        /// <param name="allowDataLoss">If true, then will make db changes even though it can result to data loss</param>
        /// <param name="targetMigration">The target migration, if null, then migrate to latest.</param>
        public void MigrateToLatestVersion(bool allowDataLoss, string targetMigration = null) {
            var cfg = new TMigrationConfig { AutomaticMigrationDataLossAllowed = allowDataLoss };
            var migrator = new DbMigrator(cfg);
            if (string.IsNullOrWhiteSpace(targetMigration)) {
                migrator.Update();
            } else {
                migrator.Update(targetMigration);
            }
        }
    }
}
