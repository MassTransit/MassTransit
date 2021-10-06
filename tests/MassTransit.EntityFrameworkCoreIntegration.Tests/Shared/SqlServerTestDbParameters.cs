namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using TestFramework;


    public class SqlServerTestDbParameters :
        ITestDbParameters
    {
        /// <summary>
        /// Get DB context options for SQL Server.
        /// </summary>
        public DbContextOptionsBuilder<T> GetDbContextOptions<T>()
            where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();

            Apply(typeof(T), builder);

            return builder;
        }

        public void Apply(Type dbContextType, DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{dbContextType.Name}");
            });
        }

        public ILockStatementProvider RawSqlLockStatements => new SqlServerLockStatementProvider(false);
    }
}
