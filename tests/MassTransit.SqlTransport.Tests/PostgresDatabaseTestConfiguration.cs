namespace MassTransit.DbTransport.Tests;

using System;
using System.Reflection;
using EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


public class PostgresDatabaseTestConfiguration :
    IDatabaseTestConfiguration
{
    public IServiceCollection Create()
    {
        return new ServiceCollection()
            .ConfigurePostgresTransport();
    }

    public ILockStatementProvider LockStatementProvider => new PostgresLockStatementProvider(false);

    public void Apply<TDbContext>(DbContextOptionsBuilder builder)
        where TDbContext : DbContext
    {
        builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;", m =>
        {
            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            m.MigrationsHistoryTable($"__{typeof(TDbContext).Name}");
        });
    }

    public void Configure(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, ISqlBusFactoryConfigurator> callback)
    {
        configurator.ConfigurePostgresTransport();

        configurator.AddConfigureEndpointsCallback((_, cfg) =>
        {
            if (cfg is ISqlReceiveEndpointConfigurator db)
                db.PurgeOnStartup = true;
        });

        configurator.UsingPostgres((context, cfg) =>
        {
            callback(context, cfg);
        });
    }
}
