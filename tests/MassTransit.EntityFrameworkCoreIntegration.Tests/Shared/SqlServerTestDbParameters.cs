namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class SqlServerTestDbParameters :
        ITestDbParameters
    {
        /// <summary>
        /// Get DB context options for SQL Server.
        /// </summary>
        /// <param name="dbContextType">Type of the DbContext, used for migration conventions</param>
        public DbContextOptionsBuilder GetDbContextOptions(Type dbContextType)
        {
            var builder = new DbContextOptionsBuilder();

            Apply(dbContextType, builder);

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
