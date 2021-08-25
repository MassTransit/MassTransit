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

    public static class OnRampConfigurationExtensions
    {
        /// <summary>
        /// Must have called AddOnRamp(...) in addition to this.
        /// </summary>
        public static IServiceCollection AddOnRampTransportHostedService(this IServiceCollection services)
        {
            services.AddHostedService<OnRampTransportHostedService>();

            return services;
        }

        public static void AddOnRampTransport(this IServiceCollection services,
        Action<OnRampTransportConfiguration> configure = null)
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
            services.TryAddScoped<IOnRampDbTransactionContext, OnRampDbTransactionContext>();
            services.TryAddScoped<IClusterRepository, SqlClusterRepository>();
            services.TryAddScoped<IOnRampTransportRepository, SqlOutboxTransportRepository>();
            services.TryAddScoped<ISweeperRepository, SqlSweeperRepository>();

            // Lock Statement Providers
            services.TryAddSingleton<IRepositoryStatementProvider, SqlServerRepositoryStatementProvider>();
            services.TryAddSingleton<IOnRampTransportRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
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
