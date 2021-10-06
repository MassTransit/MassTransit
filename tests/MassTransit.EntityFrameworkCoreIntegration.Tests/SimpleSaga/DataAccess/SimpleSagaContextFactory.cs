namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SimpleSaga.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class SimpleSagaContextFactory :
        IDesignTimeDbContextFactory<SimpleSagaDbContext>
    {
        public SimpleSagaDbContext CreateDbContext(string[] args)
        {
            // used only for database update and migrations. Since IDesignTimeDbContextFactory is icky,
            // we only support command line tools for SQL Server, so use SQL Server if you need to do
            // migrations.

            var optionsBuilder = new SqlServerTestDbParameters().GetDbContextOptions<SimpleSagaDbContext>();

            return new SimpleSagaDbContext(optionsBuilder.Options);
        }

        public SimpleSagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new SimpleSagaDbContext(optionsBuilder.Options);
        }
    }
}
