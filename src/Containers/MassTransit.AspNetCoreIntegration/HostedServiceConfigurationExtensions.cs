namespace MassTransit
{
    using System;
    using AspNetCoreIntegration;
    using AspNetCoreIntegration.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Registration;


    /// <summary>
    /// These are the updated extensions compatible with the container registration code. They should be used, for real.
    /// </summary>
    public static class HostedServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService" />, which includes a bus and endpoint health check.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services)
        {
            return services.AddMassTransitHostedService(false, null);
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService" />, which includes a bus and endpoint health check.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="waitUntilStarted">Await until bus fully started. (It will block application until bus becomes ready)</param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, bool waitUntilStarted)
        {
            return services.AddMassTransitHostedService(waitUntilStarted, null);
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService" />, which includes a bus and endpoint health check.
        /// Use it together with UseHealthCheck to get more detailed diagnostics.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="waitUntilStarted">Await until bus fully started. (It will block application until bus becomes ready)</param>
        /// <param name="startTimeout">
        /// The timeout for starting the bus. The bus start process will not respond to the hosted service's cancellation token.
        /// In other words, if host shutdown is triggered during bus startup, the startup will still complete (subject to the specified timeout).
        /// </param>
        /// <param name="stopTimeout">
        /// The timeout for stopping the bus. The bus stop process will not respond to the hosted service's cancellation token.
        /// In other words, bus shutdown will complete gracefully (subject to the specified timeout) even if instructed by ASP.NET Core
        /// to no longer be graceful.
        /// </param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, bool waitUntilStarted, TimeSpan? startTimeout,
            TimeSpan? stopTimeout = null)
        {
            AddHealthChecks(services);

            MassTransitHostedService HostedServiceFactory(IServiceProvider provider)
            {
                var busDepot = provider.GetRequiredService<IBusDepot>();
                return new MassTransitHostedService(busDepot, waitUntilStarted, startTimeout, stopTimeout);
            }

            return services.AddHostedService(HostedServiceFactory);
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus)
        {
            return services.AddMassTransitHostedService(bus, false, null);
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus, bool waitUntilStarted)
        {
            return services.AddMassTransitHostedService(bus, waitUntilStarted, null);
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus, bool waitUntilStarted,
            TimeSpan? startTimeout, TimeSpan? stopTimeout = null)
        {
            AddHealthChecks(services);

            return services.AddHostedService(_ => new BusHostedService(bus, waitUntilStarted, startTimeout, stopTimeout));
        }

        static void AddHealthChecks(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHealthChecks();
            services.AddSingleton<IConfigureOptions<HealthCheckServiceOptions>>(provider =>
                new ConfigureBusHealthCheckServiceOptions(provider.GetServices<IBusInstance>(), new[] { "ready", "masstransit" }));
        }
    }
}
