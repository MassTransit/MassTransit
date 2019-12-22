namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ITestDbContextOptionsProvider
    {
        DbContextOptionsBuilder GetDbContextOptions(Type dbContext);
    }
}
