namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbParameters
    {
        ILockStatementProvider RawSqlLockStatements { get; }

        DbContextOptionsBuilder<T> GetDbContextOptions<T>()
            where T : DbContext;

        void Apply(Type dbContextType, DbContextOptionsBuilder builder);
    }
}
