namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Testing;
    using Testing.Implementations;
    using Transports;


    public static class RabbitMqDependencyInjectionTestingExtensions
    {
        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        public static IServiceCollection AddMassTransitRabbitMqTestHarness(this IServiceCollection services,
            Action<IBusRegistrationConfigurator> configure = null)
        {
            services.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });
            services.AddSingleton(provider =>
            {
                var busInstances = provider.GetService<IEnumerable<IBusInstance>>();
                if (busInstances == null)
                {
                    var busInstance = provider.GetService<IBusInstance>();
                    busInstances = new[] { busInstance };
                }

                if (busInstances == null)
                    throw new ConfigurationException("No bus instances found");

                var testHarnessBusInstance = busInstances.FirstOrDefault(x => x is InMemoryTestHarnessBusInstance);
                if (testHarnessBusInstance is InMemoryTestHarnessBusInstance testInstance)
                    return testInstance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            });
            services.AddSingleton<BusTestHarness>(provider => provider.GetRequiredService<InMemoryTestHarness>());

            return services;
        }

    }
}
