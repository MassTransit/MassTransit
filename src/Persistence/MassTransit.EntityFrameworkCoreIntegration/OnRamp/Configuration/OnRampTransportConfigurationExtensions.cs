using MassTransit.EntityFrameworkCoreIntegration.OnRamp;
using MassTransit.Transports.OnRamp;
using MassTransit.Transports.OnRamp.Configuration;
using MassTransit.Transports.OnRamp.Repositories;
using MassTransit.Transports.OnRamp.StatementProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace MassTransit
{
    public static class OnRampTransportConfigurationExtensions
    {
        public static void AddOnRampTransport<TDbContext>(this IServiceCollection services,
            Action<OnRampTransportConfiguration> configure = null)
            where TDbContext : DbContext
        {
            // Options
            var config = new OnRampTransportConfiguration();
            configure.Invoke(config);

            services.TryAddSingleton(config.InstanceIdGenerator);
            services.TryAddSingleton<IOnRampTransportOptions>(config);
            services.TryAddSingleton<IOnRampOptions>(config);

            // All the cluster/sweeper services
            if (config.Clustered)
                services.TryAddSingleton<IOnRampSemaphore, DbSemaphore>();
            else
                services.TryAddSingleton<IOnRampSemaphore, SimpleSemaphore>();

            // Sweeper
            services.TryAddScoped<ISweeperProcessor, SweeperProcessor>();

            // Cluster Manager
            services.TryAddSingleton<OnRampInstanceState>();
            services.TryAddScoped<IClusterManager, ClusterManager>();

            // Repositories
            //services.TryAddScoped<IOutboxDbTransactionContext, OutboxDbTransactionContext>(); // don't need with Entity Framework, because the DbContext manages the transaction
            services.TryAddScoped<DbContextOnRampRepository<TDbContext>>();
            services.TryAddScoped<IClusterRepository>(p => p.GetRequiredService<DbContextOnRampRepository<TDbContext>>());
            services.TryAddScoped<IOnRampTransportRepository>(p => p.GetRequiredService<DbContextOnRampRepository<TDbContext>>());
            services.TryAddScoped<ISweeperRepository>(p => p.GetRequiredService<DbContextOnRampRepository<TDbContext>>());

            // Lock Statement Providers
            services.TryAddSingleton<IRepositoryStatementProvider, SqlRepositoryStatementProvider>();
            services.TryAddSingleton<IOnRampTransportRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ISweeperRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<IClusterRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ILockRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());

            // Schema and Table Name Providers
            services.TryAddSingleton(config.RepositoryNamingProvider);

            // Initializer
            services.TryAddScoped<IRepositoryInitializer>(p => new SqlRepositoryInitializer(p.GetRequiredService<TDbContext>().Database.GetDbConnection(), p.GetRequiredService<IRepositoryNamingProvider>()));
        }
    }
}
