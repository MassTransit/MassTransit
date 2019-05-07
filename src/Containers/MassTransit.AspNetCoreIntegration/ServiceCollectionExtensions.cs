namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using ExtensionsDependencyInjectionIntegration;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;


    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(provider =>
                {
                    var loggerFactory = provider.GetService<ILoggerFactory>();

                    if (loggerFactory != null)
                        Logger.UseLoggerFactory(loggerFactory);

                    return createBus(provider);
                });
            });

            services.AddSimplifiedHostedService();

            return services;
        }

        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <param name="configure">Use MassTransit DI extensions for IServiceCollection to register consumers and sagas</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus,
            Action<IServiceCollectionConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddMassTransit(x =>
            {
                configure(x);

                x.AddBus(provider =>
                {
                    var loggerFactory = provider.GetService<ILoggerFactory>();

                    if (loggerFactory != null)
                        Logger.UseLoggerFactory(loggerFactory);

                    return createBus(provider);
                });
            });

            services.AddSimplifiedHostedService();

            return services;
        }

        /// <summary>
        /// Register and hosts a given bus instance with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="bus">The bus instance</param>
        /// <param name="loggerFactory">ASP.NET Core logger factory instance</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, IBusControl bus, ILoggerFactory loggerFactory = null)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(provider =>
                {
                    if (loggerFactory == null)
                        loggerFactory = provider.GetService<ILoggerFactory>();

                    if (loggerFactory != null)
                        Logger.UseLoggerFactory(loggerFactory);

                    return bus;
                });
            });

            services.AddSimplifiedHostedService();

            return services;
        }

        static void AddSimplifiedHostedService(this IServiceCollection services)
        {
            var busCheck = new HealthChecks.SimplifiedBusHealthCheck();
            var receiveEndpointCheck = new HealthChecks.ReceiveEndpointHealthCheck();

            services.AddHealthChecks()
                .AddBusHealthCheck("bus", busCheck)
                .AddBusHealthCheck("endpoint", receiveEndpointCheck);

            services.AddSingleton<IHostedService>(p =>
            {
                var bus = p.GetRequiredService<IBusControl>();
                var loggerFactory = p.GetService<ILoggerFactory>();

                return new MassTransitHostedService(bus, loggerFactory, busCheck, receiveEndpointCheck);
            });
        }

        static IHealthChecksBuilder AddBusHealthCheck(this IHealthChecksBuilder builder, string suffix, IHealthCheck healthCheck)
        {
            return builder.AddCheck($"masstransit-{suffix}", healthCheck, HealthStatus.Unhealthy, new[] {"ready"});
        }
    }
}
