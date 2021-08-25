namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using TestFramework;


    public class SqlServerResiliencyTestDbParameters :
        ITestDbParameters
    {
        /// <summary>
        /// Get DB context options for SQL Server, with resiliency
        /// </summary>
        /// <param name="dbContextType">Type of the DbContext, used for migration conventions</param>
        public DbContextOptionsBuilder GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext
        {
            var builder = new DbContextOptionsBuilder();

            Apply<TDbContext>(builder);

            return builder;
        }

        public void Apply<TDbContext>(DbContextOptionsBuilder builder)
            where TDbContext : DbContext
        {
            builder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{typeof(TDbContext).Name}");
                m.EnableRetryOnFailure();
            });
        }

        public ILockStatementProvider RawSqlLockStatements => new SqlServerLockStatementProvider(false);
    }
}
