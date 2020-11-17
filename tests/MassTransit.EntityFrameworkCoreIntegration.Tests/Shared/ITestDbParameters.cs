namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbParameters
    {
        ILockStatementProvider RawSqlLockStatements { get; }
        DbContextOptionsBuilder<TDbContext> GetDbContextOptions<TDbContext>()
            where TDbContext : DbContext;

        void Apply<TDbContext>(DbContextOptionsBuilder<TDbContext> builder)
            where TDbContext : DbContext;
    }
}
