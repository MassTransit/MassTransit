namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class PostgresTestDbParameters : ITestDbParameters
    {
        public DbContextOptionsBuilder GetDbContextOptions(Type dbContextType)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();

            dbContextOptionsBuilder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;",
                m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{dbContextType.Name}");
                });

            return dbContextOptionsBuilder;
        }

        public ILockStatementProvider RawSqlLockStatements => new PostgresLockStatementProvider(false);
    }
}
