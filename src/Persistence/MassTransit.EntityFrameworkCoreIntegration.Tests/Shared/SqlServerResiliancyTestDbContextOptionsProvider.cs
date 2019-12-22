namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class SqlServerResiliancyTestDbContextOptionsProvider : ITestDbContextOptionsProvider
    {
        /// <summary>
        /// Get DB context options for SQL Server, with resiliancy
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
                    m.EnableRetryOnFailure();
                });

            return dbContextOptionsBuilder;
        }
    }
}
