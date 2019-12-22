namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency.DataAccess
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Internal;
    using Shared;


    public class SagaWithDependencyContextFactory :
        IDesignTimeDbContextFactory<SagaWithDependencyContext>
    {
        public SagaWithDependencyContext CreateDbContext(string[] args)
        {
            // used only for database update and migrations. Since IDesignTimeDbContextFactory is icky,
            // we only support command line tools for SQL Server, so use SQL Server if you need to do
            // migrations.

            var optionsBuilder = new SqlServerTestDbContextOptionsProvider().GetDbContextOptions(typeof(SagaWithDependencyContext));

            return new SagaWithDependencyContext(optionsBuilder.Options);
        }

        public SagaWithDependencyContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new SagaWithDependencyContext(optionsBuilder.Options);
        }
    }
}
