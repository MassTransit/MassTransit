#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public static class MassTransitHealthCheckOptionsExtensions
    {
        /// <summary>
        /// Configure the health check options for this bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IBusRegistrationConfigurator ConfigureHealthCheckOptions(this IBusRegistrationConfigurator configurator,
            Action<IHealthCheckOptionsConfigurator>? callback)
        {
            configurator.AddOptions<MassTransitHealthCheckOptions<IBus>>()
                .Configure(options =>
                {
                    callback?.Invoke(options);
                });

            return configurator;
        }

        /// <summary>
        /// Configure the health check options for this bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IBusRegistrationConfigurator ConfigureHealthCheckOptions<T>(this IBusRegistrationConfigurator<T> configurator,
            Action<IHealthCheckOptionsConfigurator>? callback)
            where T : class, IBus
        {
            configurator.AddOptions<MassTransitHealthCheckOptions<T>>()
                .Configure(options =>
                {
                    callback?.Invoke(options);
                });

            return configurator;
        }
    }
}
