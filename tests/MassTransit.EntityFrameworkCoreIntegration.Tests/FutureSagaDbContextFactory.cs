namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;
    using MassTransit.Tests;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using TestFramework;

    public class FutureSagaDbContextFactory :
        IDesignTimeDbContextFactory<FutureSagaDbContext>
    {
        public FutureSagaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FutureSagaDbContext>();

            Apply(builder);

            return new FutureSagaDbContext(builder.Options);
        }

        public static void Apply(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{nameof(FutureSagaDbContext)}");
            });
        }

        public FutureSagaDbContext CreateDbContext(DbContextOptionsBuilder<FutureSagaDbContext> optionsBuilder)
        {
            return new FutureSagaDbContext(optionsBuilder.Options);
        }
    }
}
