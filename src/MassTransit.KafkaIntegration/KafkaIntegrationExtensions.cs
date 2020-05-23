namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public static class KafkaIntegrationExtensions
    {
        public static void AttachKafka(this IServiceCollectionConfigurator configurator, Action<IRegistration, IKafkaFactoryConfigurator> configure)
        {
            configurator.Collection.AddSingleton(provider =>
            {
                var registration = provider.GetRequiredService<IRegistration>();
                var factoryConfigurator = new KafkaFactoryConfigurator(provider.GetService<ClientConfig>() ?? new ClientConfig());
                configure?.Invoke(registration, factoryConfigurator);
                return factoryConfigurator.Build();
            });
        }
    }
}
