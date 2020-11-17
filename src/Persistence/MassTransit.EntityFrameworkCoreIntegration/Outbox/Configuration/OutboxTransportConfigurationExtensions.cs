using MassTransit.EntityFrameworkCoreIntegration.Outbox;
using MassTransit.Transports.Outbox;
using MassTransit.Transports.Outbox.Configuration;
using MassTransit.Transports.Outbox.Repositories;
using MassTransit.Transports.Outbox.StatementProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace MassTransit
{
    public static class OutboxTransportConfigurationExtensions
    {
        public static void AddOutboxTransport<TDbContext>(this IServiceCollection services,
            Action<OutboxTransportConfiguration> configure = null)
            where TDbContext : DbContext
        {
            // Options
            var config = new OutboxTransportConfiguration();
            configure.Invoke(config);

            services.TryAddSingleton(config.InstanceIdGenerator);
            services.TryAddSingleton<IOutboxTransportOptions>(config);
            services.TryAddSingleton<IOutboxOptions>(config);

            // All the cluster/sweeper services
            if (config.Clustered)
                services.TryAddSingleton<IOutboxSemaphore, DbSemaphore>();
            else
                services.TryAddSingleton<IOutboxSemaphore, SimpleSemaphore>();

            // Sweeper
            services.TryAddScoped<ISweeperProcessor, SweeperProcessor>();

            // Cluster Manager
            services.TryAddSingleton<OutboxInstanceState>();
            services.TryAddScoped<IClusterManager, ClusterManager>();

            // Repositories
            //services.TryAddScoped<IOutboxDbTransactionContext, OutboxDbTransactionContext>(); // don't need with Entity Framework, because the DbContext manages the transaction
            services.TryAddScoped<DbContextOutboxRepository<TDbContext>>();
            services.TryAddScoped<IClusterRepository>(p => p.GetRequiredService<DbContextOutboxRepository<TDbContext>>());
            services.TryAddScoped<IOutboxTransportRepository>(p => p.GetRequiredService<DbContextOutboxRepository<TDbContext>>());
            services.TryAddScoped<ISweeperRepository>(p => p.GetRequiredService<DbContextOutboxRepository<TDbContext>>());

            // Lock Statement Providers
            services.TryAddSingleton<IRepositoryStatementProvider, SqlFormatItemRepositoryStatementProvider>();
            services.TryAddSingleton<IOutboxTransportRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ISweeperRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<IClusterRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ILockRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());

            // Schema and Table Name Providers
            services.TryAddSingleton(config.RepositoryNamingProvider);

            // Initializer
            services.TryAddScoped<IRepositoryInitializer>(p => new SqlServerRepositoryInitializer(p.GetRequiredService<TDbContext>().Database.GetDbConnection(), p.GetRequiredService<IRepositoryNamingProvider>()));
        }
    }
}
