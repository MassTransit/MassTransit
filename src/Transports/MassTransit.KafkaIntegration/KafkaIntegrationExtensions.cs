namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;


    public static class KafkaIntegrationExtensions
    {
        public static void UsingKafka<TContainerContext>(this IBusAttachmentRegistrationConfigurator<TContainerContext> configurator,
            Action<IBusAttachmentRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            var factory = new KafkaBusInstanceConfiguratorFactory<TContainerContext>(configure);
            configurator.SetBusAttachmentFactory(factory);
        }

        public static void UsingKafka<TContainerContext>(this IBusAttachmentRegistrationConfigurator<TContainerContext> configurator,
            ClientConfig clientConfig,
            Action<IBusAttachmentRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            var factory = new KafkaBusInstanceConfiguratorFactory<TContainerContext>(clientConfig, configure);
            configurator.SetBusAttachmentFactory(factory);
        }
    }
}
