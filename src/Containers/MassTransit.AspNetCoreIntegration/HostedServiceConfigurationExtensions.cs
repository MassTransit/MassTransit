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
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection collection)
        {
            var receiveEndpointCheck = collection.AddAndGetHealthChecks();

            return collection.AddSingleton<IHostedService>(p =>
            {
                var bus = p.GetRequiredService<IBusControl>();

                return new MassTransitHostedService(bus, receiveEndpointCheck);
            });
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService"/>, which includes a bus and endpoint health check, for a specific bus instance.
        /// Use it together with <see cref="UseHealthCheck"/> to get more detailed diagnostics.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="bus">Configured bus instance</param>
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection collection, IBusControl bus)
        {
            var receiveEndpointCheck = collection.AddAndGetHealthChecks();

            return collection.AddSingleton<IHostedService>(new MassTransitHostedService(bus, receiveEndpointCheck));
        }

        static ReceiveEndpointHealthCheck AddAndGetHealthChecks(this IServiceCollection collection)
        {
            var busCheck = new BusHealthCheck();
            var receiveEndpointCheck = new ReceiveEndpointHealthCheck();

            collection.AddSingleton(busCheck);

            collection.AddHealthChecks()
                .AddBusHealthCheck("bus", busCheck)
                .AddBusHealthCheck("endpoint", receiveEndpointCheck);

            return receiveEndpointCheck;
        }

        static IHealthChecksBuilder AddBusHealthCheck(this IHealthChecksBuilder builder, string suffix, IHealthCheck healthCheck)
        {
            return builder.AddCheck($"masstransit-{suffix}", healthCheck, HealthStatus.Unhealthy, new[] {"ready"});
        }
    }
}
