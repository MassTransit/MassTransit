namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Diagnostics;
    using Context;
    using ExtensionsDependencyInjectionIntegration;
    using HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;


    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <param name="configureHealthChecks">Optional, allows you to specify custom health check names</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus,
            Action<HealthCheckOptions> configureHealthChecks = null)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(provider =>
                {
                    ConfigureLogging(provider);

                    return createBus(provider);
                });
            });

            services.AddSimplifiedHostedService(configureHealthChecks);

            return services;
        }

        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <param name="configure">Use MassTransit DI extensions for IServiceCollection to register consumers and sagas</param>
        /// <param name="configureHealthChecks">Optional, allows you to specify custom health check names</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus,
            Action<IServiceCollectionConfigurator> configure, Action<HealthCheckOptions> configureHealthChecks = null)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddMassTransit(x =>
            {
                configure(x);

                x.AddBus(provider =>
                {
                    ConfigureLogging(provider);

                    return createBus(provider);
                });
            });

            services.AddSimplifiedHostedService(configureHealthChecks);

            return services;
        }

        /// <summary>
        /// Register and hosts a given bus instance with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="bus">The bus instance</param>
        /// <param name="loggerFactory">Optional: ASP.NET Core logger factory instance</param>
        /// <param name="configureHealthChecks">Optional, allows you to specify custom health check names</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, IBusControl bus, ILoggerFactory loggerFactory = null,
            Action<HealthCheckOptions> configureHealthChecks = null)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(provider =>
                {
                    ConfigureLogging(provider, loggerFactory);

                    return bus;
                });
            });

            services.AddSimplifiedHostedService(configureHealthChecks);

            return services;
        }

        static void ConfigureLogging(IServiceProvider provider, ILoggerFactory loggerFactory = null)
        {
            if (loggerFactory == null)
                loggerFactory = provider.GetService<ILoggerFactory>();
            var diagnosticSource = provider.GetService<DiagnosticSource>();

            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory, diagnosticSource);
        }

        static void AddSimplifiedHostedService(this IServiceCollection services, Action<HealthCheckOptions> configureHealthChecks)
        {
            var busCheck = new SimplifiedBusHealthCheck();
            var receiveEndpointCheck = new ReceiveEndpointHealthCheck();

            var healthCheckOptions = HealthCheckOptions.Default;
            configureHealthChecks?.Invoke(healthCheckOptions);

            services.AddHealthChecks()
                .AddCheck(healthCheckOptions.BusHealthCheckName, busCheck, healthCheckOptions.FailureStatus, healthCheckOptions.Tags)
                .AddCheck(healthCheckOptions.ReceiveEndpointHealthCheckName, receiveEndpointCheck, healthCheckOptions.FailureStatus, healthCheckOptions.Tags);

            services.AddSingleton<IHostedService>(p =>
            {
                var bus = p.GetRequiredService<IBusControl>();

                return new MassTransitHostedService(bus, busCheck, receiveEndpointCheck);
            });
        }
    }
}
