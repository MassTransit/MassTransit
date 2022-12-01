#nullable enable
namespace MassTransit
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Testing;


    public static class ServiceBusDependencyInjectionTestingExtensions
    {
        /// <summary>
        /// Specify the test and/or the test inactivity timeouts that should be used by the test harness.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServiceBusTestOptions(this IServiceCollection services, Action<AzureServiceBusTestHarnessOptions>? configure)
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(MassTransitHostedService));
            if (descriptor != null)
                throw new ConfigurationException("Azure Service Bus Test Options must be configured before calling AddMassTransit");

            services.AddOptions<AzureServiceBusTestHarnessOptions>()
                .Configure(options =>
                {
                    configure?.Invoke(options);
                });

            services.AddHostedService<AzureServiceBusTestHarnessHostedService>();

            return services;
        }
    }
}
