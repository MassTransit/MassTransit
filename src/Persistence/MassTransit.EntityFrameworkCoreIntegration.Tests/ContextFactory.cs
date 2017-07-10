namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Reflection;

    using MassTransit.Tests.Saga;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public class ContextFactory : IDbContextFactory<SagaDbContext<SimpleSaga, SimpleSagaMap>>
    {
        public SagaDbContext<SimpleSaga, SimpleSagaMap> Create(DbContextFactoryOptions options)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SagaDbContext<SimpleSaga, SimpleSagaMap>>();
            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m => m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

            return new SagaDbContext<SimpleSaga, SimpleSagaMap>(dbContextOptionsBuilder.Options);
        }
    }
}