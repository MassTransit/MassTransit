namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class SqlServerTestDbParameters : ITestDbParameters
    {
        /// <summary>
        /// Get DB context options for SQL Server.
        /// </summary>
        /// <param name="dbContextType">Type of the dbcontext, used for migration conventions</param>
        public DbContextOptionsBuilder GetDbContextOptions(Type dbContextType)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();

            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{dbContextType.Name}");
                });

            return dbContextOptionsBuilder;
        }

        public ILockStatementProvider RawSqlLockStatements => new SqlServerLockStatementProvider(false);
    }
}
