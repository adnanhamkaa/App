using App.DataAccess.Identity;
using App.DataAccess.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;
using System.Data.Entity;

namespace App.DataAccess
{

    public class NpgSqlConfiguration : DbConfiguration {
        public NpgSqlConfiguration() {
            var name = "Npgsql";

            SetProviderFactory(providerInvariantName: name,
            providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(providerInvariantName: name,
            provider: NpgsqlServices.Instance);
            
            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
    }

    [DbConfigurationType(typeof(App.DataAccess.NpgSqlConfiguration))]
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> {
        public ApplicationDbContext() : base(nameOrConnectionString: "DefaultConnection") { }
        public ApplicationDbContext(string connStr) : base(nameOrConnectionString: connStr) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(21, 3));
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
            
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<ActionAuthorization> ActionAuthorization { get; set; }
        public DbSet<SynchronizerLog> SynchronizerLog { get; set; }
        public DbSet<KeyValueStore> KeyValueStore { get; set; }

        
        #region Sample Model
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        #endregion Sample Model
    }

}

