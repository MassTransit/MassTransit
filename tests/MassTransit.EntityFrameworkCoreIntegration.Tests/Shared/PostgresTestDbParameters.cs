namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class PostgresTestDbParameters :
        ITestDbParameters
    {
        public DbContextOptionsBuilder GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();

            Apply<TDbContext>(builder);

            return builder;
        }

        public void Apply<TDbContext>(DbContextOptionsBuilder builder)
            where TDbContext : DbContext
        {
            builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;", m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{typeof(TDbContext).Name}");
            });
        }

        public ILockStatementProvider RawSqlLockStatements => new PostgresLockStatementProvider(false);
    }
}
