namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbParameters
    {
        ILockStatementProvider RawSqlLockStatements { get; }
        DbContextOptionsBuilder GetDbContextOptions(Type dbContextType);

        void Apply(Type dbContextType, DbContextOptionsBuilder builder);
    }
}
