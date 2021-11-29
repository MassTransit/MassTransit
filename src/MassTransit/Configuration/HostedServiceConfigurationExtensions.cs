namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// These are the updated extensions compatible with the container registration code. They should be used, for real.
    /// </summary>
    public static class HostedServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds the MassTransit <see cref="Microsoft.Extensions.Hosting.IHostedService" />, which includes a bus and endpoint health check.
        /// </summary>
        /// <param name="services"></param>
        [Obsolete("Deprecated, hosted service is automatically added to the container")]
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services)
        {
            services.AddOptions<MassTransitHostOptions>();

            return services;
        }

        /// <summary>
        /// Adds the MassTransit <see cref="Microsoft.Extensions.Hosting.IHostedService" />, which includes a bus and endpoint health check.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="waitUntilStarted">Await until bus fully started. (It will block application until bus becomes ready)</param>
        [Obsolete("Deprecated, hosted service is automatically added to the container. Configure MassTransitHostOptions to modify the default options.")]
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, bool waitUntilStarted)
        {
            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = waitUntilStarted;
                });

            return services;
        }

        /// <summary>
        /// Adds the MassTransit <see cref="Microsoft.Extensions.Hosting.IHostedService" />, which includes a bus and endpoint health check.
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
        [Obsolete("Deprecated, hosted service is automatically added to the container. Configure MassTransitHostOptions to modify the default options.")]
        public static IServiceCollection AddMassTransitHostedService(this IServiceCollection services, bool waitUntilStarted, TimeSpan? startTimeout,
            TimeSpan? stopTimeout = null)
        {
            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = waitUntilStarted;
                    options.StartTimeout = startTimeout;
                    options.StopTimeout = stopTimeout;
                });

            return services;
        }
    }
}
