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
        /// Uses the health check, in combination with the MassTransit Hosted Service, to monitor the bus
        /// </summary>
        /// <param name="busConfigurator"></param>
        /// <param name="provider"></param>
        public static void UseHealthCheck(this IBusFactoryConfigurator busConfigurator, IServiceProvider provider)
        {
            var healthCheck = provider.GetRequiredService<BusHealthCheck>();

            busConfigurator.ConnectBusObserver(healthCheck);
        }

        /// <summary>
        /// Adds the MassTransit <see cref="IHostedService"/>, which includes a bus and endpoint health check
        /// </summary>
        /// <param name="collection"></param>
        public static void AddMassTransitHostedService(this IServiceCollection collection)
        {
            var busCheck = new BusHealthCheck();
            var receiveEndpointCheck = new ReceiveEndpointHealthCheck();

            collection.AddSingleton(busCheck);

            collection.AddHealthChecks()
                .AddBusHealthCheck("bus", busCheck)
                .AddBusHealthCheck("endpoint", receiveEndpointCheck);

            collection.AddSingleton<IHostedService>(p =>
            {
                var bus = p.GetRequiredService<IBusControl>();

                return new MassTransitHostedService(bus, new SimplifiedBusHealthCheck(), receiveEndpointCheck);
            });
        }

        static IHealthChecksBuilder AddBusHealthCheck(this IHealthChecksBuilder builder, string suffix, IHealthCheck healthCheck)
        {
            return builder.AddCheck($"masstransit-{suffix}", healthCheck, HealthStatus.Unhealthy, new[] {"ready"});
        }
    }
}
