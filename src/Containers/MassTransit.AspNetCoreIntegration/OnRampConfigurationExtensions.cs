namespace MassTransit
{
    using AspNetCoreIntegration;
    using MassTransit.Transports.OnRamp;
    using MassTransit.Transports.OnRamp.Configuration;
    using MassTransit.Transports.OnRamp.Repositories;
    using MassTransit.Transports.OnRamp.StatementProviders;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;

    public static class OnRampConfigurationExtensions
    {
        /// <summary>
        /// Must have called AddOnRampTransport(...) in addition to this.
        /// </summary>
        public static IServiceCollection AddOnRampTransportHostedService(this IServiceCollection services)
        {
            services.AddHostedService<OnRampTransportHostedService>();

            return services;
        }

        /// <summary>
        /// Adds OnRamp Transport using System.Data to allow support for any Relational Database. Db type can
        /// be specified with cfg.Use...  If none is specified, then Sqlite is the default
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
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
            services.TryAddScoped<IOnRampTransportRepository, SqlOnRampTransportRepository>();
            services.TryAddScoped<ISweeperRepository, SqlSweeperRepository>();

            // Schema and Table Name Providers
            var provider = config.RepositoryNamingProvider ?? new SqliteRepositoryNamingProvider("mt");
            services.TryAddSingleton(provider);

            switch (provider)
            {
                // SQL Server has some special exceptions, like TOP instead of LIMIT, and UPD/ROW lock statements
                case SqlServerRepositoryNamingProvider:
                    services.TryAddSingleton<IRepositoryStatementProvider, SqlServerRepositoryStatementProvider>();
                    break;
                default:
                    services.TryAddSingleton<IRepositoryStatementProvider, SqlRepositoryStatementProvider>();
                    break;
            }

            // Statement Providers
            services.TryAddSingleton<IOnRampTransportRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ISweeperRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<IClusterRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());
            services.TryAddSingleton<ILockRepositoryStatementProvider>(p => p.GetRequiredService<IRepositoryStatementProvider>());

            

            // Initializer
            services.TryAddScoped<IRepositoryInitializer, SqlRepositoryInitializer>();
        }
    }
}
