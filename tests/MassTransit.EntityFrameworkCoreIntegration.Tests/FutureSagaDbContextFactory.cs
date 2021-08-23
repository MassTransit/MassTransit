namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using TestFramework;


    public class FutureSagaDbContextFactory :
        IDesignTimeDbContextFactory<FutureSagaDbContext>
    {
        public FutureSagaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder();

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

        public FutureSagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new FutureSagaDbContext(optionsBuilder.Options);
        }
    }
}
