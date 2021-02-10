namespace MassTransit
{
    using AspNetCoreIntegration;
    using MassTransit.Transports.Outbox;
    using MassTransit.Transports.Outbox.Configuration;
    using MassTransit.Transports.Outbox.Repositories;
    using MassTransit.Transports.Outbox.StatementProviders;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;

    public static class OutboxConfigurationExtensions
    {
        /// <summary>
        /// Must have called AddOutboxTransport(...) in addition to this.
        /// </summary>
        public static IServiceCollection AddOutboxTransportHostedService(this IServiceCollection services)
        {
            services.AddHostedService<OutboxTransportHostedService>();

            return services;
        }

        public static void AddOutboxTransport(this IServiceCollection services,
        Action<OutboxTransportConfiguration> configure = null)
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
            services.TryAddScoped<IOutboxDbTransactionContext, OutboxDbTransactionContext>();
            services.TryAddScoped<IClusterRepository, SqlClusterRepository>();
            services.TryAddScoped<IOutboxTransportRepository, SqlOutboxTransportRepository>();
            services.TryAddScoped<ISweeperRepository, SqlSweeperRepository>();

            // Lock Statement Providers
            services.TryAddSingleton<IRepositoryStatementProvider, SqlServerRepositoryStatementProvider>();
            services.TryAddSingleton<IOutboxTransportRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ISweeperRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<IClusterRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ILockRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());

            // Schema and Table Name Providers
            services.TryAddSingleton(config.RepositoryNamingProvider);

            // Initializer
            services.TryAddScoped<IRepositoryInitializer, SqlServerRepositoryInitializer>();
        }
    }
}
