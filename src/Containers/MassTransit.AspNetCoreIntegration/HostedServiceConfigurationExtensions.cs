namespace MassTransit
{
    using System;
    using AspNetCoreIntegration;
    using AspNetCoreIntegration.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;


    /// <summary>
    /// These are the updated extensions compatible with the container registration code. They should be used, for real.
    /// </summary>
    public static class HostedServiceConfigurationExtensions
    {
        /// <summary>
        /// Uses the health check, in combination with the MassTransit Hosted Service (<see cref="AddMassTransitHostedService"/>), to monitor the bus
        /// </summary>
        /// <param name="busConfigurator"></param>
        /// <param name="provider"></param>
        public static void UseHealthCheck(this IBusFactoryConfigurator busConfigurator, IServiceProvider provider)
        {
            var healthCheck = provider.GetRequiredService<BusHealthCheck>();

            busConfigurator.ConnectBusObserver(healthCheck);
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService"/>, which includes a bus and endpoint health check.
        /// Use it together with <see cref="UseHealthCheck"/> to get more detailed diagnostics.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="bus">The bus instance to add. If not specified, the bus will be resolved from the service collection.</param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection collection, IBusControl bus = null)
        {
            var endpointHealthCheck = collection.AddAndGetHealthChecks();

            return collection.AddSingleton<IHostedService>(p => new MassTransitHostedService(bus ?? p.GetRequiredService<IBusControl>(), endpointHealthCheck));
        }

        static ReceiveEndpointHealthCheck AddAndGetHealthChecks(this IServiceCollection collection)
        {
            var busHealthCheck = new BusHealthCheck();
            var endpointHealthCheck = new ReceiveEndpointHealthCheck();

            collection.AddSingleton(busHealthCheck);
            collection.AddHealthChecks()
                .AddBusHealthCheck("bus", busHealthCheck)
                .AddBusHealthCheck("endpoint", endpointHealthCheck);

            return endpointHealthCheck;
        }

        static IHealthChecksBuilder AddBusHealthCheck(this IHealthChecksBuilder builder, string suffix, IHealthCheck healthCheck)
        {
            return builder.AddCheck($"masstransit-{suffix}", healthCheck, HealthStatus.Unhealthy, new[] {"ready"});
        }
    }
}
