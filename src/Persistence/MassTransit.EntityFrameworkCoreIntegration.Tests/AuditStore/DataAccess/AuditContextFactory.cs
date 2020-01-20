namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;

    using MassTransit.EntityFrameworkCoreIntegration.Audit;
    using MassTransit.Tests.AutomatonymousIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class AuditContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
    {
        public AuditDbContext CreateDbContext(string[] args)
        {
            // used only for database update and migrations. Since IDesignTimeDbContextFactory is icky,
            // we only support command line tools for SQL Server, so use SQL Server if you need to do
            // migrations.

            var optionsBuilder = new SqlServerTestDbParameters().GetDbContextOptions(typeof(AuditDbContext));
            return CreateDbContext(optionsBuilder);
        }

        public AuditDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new AuditDbContext(optionsBuilder.Options, "EfCoreAudit");
        }
    }
}
