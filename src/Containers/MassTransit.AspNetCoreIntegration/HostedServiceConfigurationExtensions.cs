namespace MassTransit
{
    using System;
    using AspNetCoreIntegration;
    using AspNetCoreIntegration.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Monitoring.Health;
    using Registration;


    /// <summary>
    /// These are the updated extensions compatible with the container registration code. They should be used, for real.
    /// </summary>
    public static class HostedServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService" />, which includes a bus and endpoint health check.
        /// Use it together with UseHealthCheck to get more detailed diagnostics.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services)
        {
            return services.AddMassTransitHostedService(false);
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService" />, which includes a bus and endpoint health check.
        /// Use it together with UseHealthCheck to get more detailed diagnostics.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="waitUntilStarted">Await until bus fully started. (It will block application until bus becomes ready)</param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, bool waitUntilStarted)
        {
            AddHealthChecks(services);

            MassTransitHostedService HostedServiceFactory(IServiceProvider provider)
            {
                var busDepot = provider.GetRequiredService<IBusDepot>();
                return new MassTransitHostedService(busDepot, waitUntilStarted);
            }

            return services.AddHostedService(HostedServiceFactory);
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus)
        {
            return services.AddMassTransitHostedService(bus, false);
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus, bool waitUntilStarted)
        {
            AddHealthChecks(services);

            return services.AddHostedService(_ => new BusHostedService(bus, waitUntilStarted));
        }

        static void AddHealthChecks(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHealthChecks();
            services.AddSingleton<IConfigureOptions<HealthCheckServiceOptions>>(provider =>
                new ConfigureBusHealthCheckServiceOptions(provider.GetServices<IBusHealth>(), new[] {"ready", "masstransit"}));
        }
    }
}
