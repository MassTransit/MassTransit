namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using KafkaIntegration;
    using KafkaIntegration.Registration;
    using Registration;


    public static class KafkaIntegrationExtensions
    {
        public static void UsingKafka<TContainerContext>(this IRiderConfigurator<TContainerContext> configurator,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaRegistrationRiderFactory<TContainerContext>(configure);
            configurator.SetRiderFactory(factory);
        }

        public static void UsingKafka<TContainerContext>(this IRiderConfigurator<TContainerContext> configurator,
            ClientConfig clientConfig,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaRegistrationRiderFactory<TContainerContext>(clientConfig, configure);
            configurator.SetRiderFactory(factory);
        }
    }
}
