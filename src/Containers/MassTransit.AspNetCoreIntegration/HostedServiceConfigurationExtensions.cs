namespace MassTransit
{
    using AspNetCoreIntegration;
    using AspNetCoreIntegration.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Monitoring.Health;


    /// <summary>
    /// These are the updated extensions compatible with the container registration code. They should be used, for real.
    /// </summary>
    public static class HostedServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService"/>, which includes a bus and endpoint health check.
        /// Use it together with UseHealthCheck to get more detailed diagnostics.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services)
        {
            AddHealthChecks(services);

            return services.AddSingleton<IHostedService, MassTransitHostedService>();
        }

        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, IBusControl bus)
        {
            AddHealthChecks(services);

            return services.AddSingleton<IHostedService>(p => new BusHostedService(bus));
        }

        static void AddHealthChecks(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHealthChecks();
            services.AddSingleton<IConfigureOptions<HealthCheckServiceOptions>>(provider =>
                new ConfigureBusHealthCheckServiceOptions(provider.GetServices<IBusHealth>(), new[] {"ready"}));
        }
    }
}
