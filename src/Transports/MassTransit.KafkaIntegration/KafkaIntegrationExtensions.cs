namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;


    public static class KafkaIntegrationExtensions
    {
        public static void UsingKafka<TContainerContext>(this IRiderRegistrationConfigurator<TContainerContext> configurator,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaBusInstanceConfiguratorFactory<TContainerContext>(configure);
            configurator.SetRiderFactory(factory);
        }

        public static void UsingKafka<TContainerContext>(this IRiderRegistrationConfigurator<TContainerContext> configurator,
            ClientConfig clientConfig,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaBusInstanceConfiguratorFactory<TContainerContext>(clientConfig, configure);
            configurator.SetRiderFactory(factory);
        }
    }
}
