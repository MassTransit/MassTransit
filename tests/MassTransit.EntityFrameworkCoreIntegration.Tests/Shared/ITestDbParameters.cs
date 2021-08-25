namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbParameters
    {
        ILockStatementProvider RawSqlLockStatements { get; }
        DbContextOptionsBuilder GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext;

        void Apply<TDbContext>(DbContextOptionsBuilder builder)
            where TDbContext : DbContext;
    }
}
