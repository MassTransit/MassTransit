namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbParameters
    {
        DbContextOptionsBuilder GetDbContextOptions(Type dbContextType);

        ILockStatementProvider RawSqlLockStatements { get; }
    }
}
