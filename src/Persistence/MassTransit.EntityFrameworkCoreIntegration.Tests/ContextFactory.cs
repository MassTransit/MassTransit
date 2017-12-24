namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;

    using MassTransit.Tests.Saga;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;


    public class ContextFactory : IDesignTimeDbContextFactory<SagaDbContext<SimpleSaga, SimpleSagaMap>>
    {
        public SagaDbContext<SimpleSaga, SimpleSagaMap> CreateDbContext(string[] args)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SagaDbContext<SimpleSaga, SimpleSagaMap>>();

            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m =>
                    {
                        var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;
                        m.MigrationsAssembly(executingAssembly.GetName().Name);
                    });

            return new SagaDbContext<SimpleSaga, SimpleSagaMap>(dbContextOptionsBuilder.Options);
        }
    }
}