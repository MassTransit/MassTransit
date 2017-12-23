namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;

    using MassTransit.EntityFrameworkCoreIntegration.Audit;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class AuditContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
    {
        public AuditDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>().
                UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                    m =>
                        {
                            var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;

                            m.MigrationsAssembly(executingAssembly.GetName().Name);
                            m.MigrationsHistoryTable("__AuditEFMigrationHistoryAudit");
                        });

            return new AuditDbContext(optionsBuilder.Options, "EfCoreAudit");

        }
    }
}