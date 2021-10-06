namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;


    public class PostgresTestDbParameters :
        ITestDbParameters
    {
        public DbContextOptionsBuilder<T> GetDbContextOptions<T>()
            where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();

            Apply(typeof(T), builder);

            return builder;
        }

        public void Apply(Type dbContextType, DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;", m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                m.MigrationsHistoryTable($"__{dbContextType.Name}");
            });
        }

        public ILockStatementProvider RawSqlLockStatements => new PostgresLockStatementProvider(false);
    }
}
