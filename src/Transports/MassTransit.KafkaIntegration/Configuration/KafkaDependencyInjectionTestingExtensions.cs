#nullable enable
namespace MassTransit
{
    using System;
    using System.Linq;
    using Confluent.Kafka;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Testing;


    public static class KafkaDependencyInjectionTestingExtensions
    {
        /// <summary>
        /// Specify kafka topic options behavior for tests
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clientConfig"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureKafkaTestOptions(this IServiceCollection services, ClientConfig clientConfig,
            Action<KafkaTestHarnessOptions>? configure = null)
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(MassTransitHostedService));
            if (descriptor != null)
                throw new ConfigurationException("Kafka Test Options must be configured before calling AddMassTransit");

            static IAdminClient CreateClient(IServiceProvider provider)
            {
                return new AdminClientBuilder(new AdminClientConfig(provider.GetRequiredService<ClientConfig>())).Build();
            }

            services
                .AddSingleton(CreateClient)
                .AddSingleton(clientConfig)
                .AddOptions<KafkaTestHarnessOptions>()
                .Configure(options =>
                {
                    configure?.Invoke(options);
                });

            services.AddHostedService<KafkaTestHarnessHostedService>();

            return services;
        }

        /// <summary>
        /// Specify kafka topic options behavior for tests by using default host and port
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureKafkaTestOptions(this IServiceCollection services,
            Action<KafkaTestHarnessOptions>? configure = null)
        {
            var host = HostMetadataCache.IsRunningInContainer ? "broker" : "localhost";
            var clientConfig = new ClientConfig { BootstrapServers = $"{host}:9092" };
            return ConfigureKafkaTestOptions(services, clientConfig, configure);
        }
    }
}
