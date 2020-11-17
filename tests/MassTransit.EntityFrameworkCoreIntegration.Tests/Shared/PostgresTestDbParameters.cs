namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class PostgresTestDbParameters :
        ITestDbParameters
    {
        public DbContextOptionsBuilder<TDbContext> GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();

            Apply(builder);

            return builder;
        }

        public void Apply<TDbContext>(DbContextOptionsBuilder<TDbContext> builder)
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
