namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;


    public class SagaWithDependencyContextFactory : 
        IDesignTimeDbContextFactory<SagaWithDependencyContext>
    {
        public SagaWithDependencyContext CreateDbContext(string[] args)
        {
            var dbContextOptionsBuilder = 
                new DbContextOptionsBuilder<SagaWithDependencyContext>();

            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m =>
                {
                    var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;
                    m.MigrationsAssembly(executingAssembly.GetName().Name);
                    m.MigrationsHistoryTable("__SagaWithDependencyMigrations");
                });

            return new SagaWithDependencyContext(dbContextOptionsBuilder.Options);
        }
    }
}