namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using KafkaIntegration;
    using KafkaIntegration.Registration;
    using KafkaIntegration.Transport;
    using Registration;


    public static class KafkaIntegrationExtensions
    {
        public static void UsingKafka<TContainerContext>(this IRiderConfigurator<TContainerContext> configurator,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            RegisterComponents(configurator.Registrar);

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
            if (clientConfig == null)
                throw new ArgumentNullException(nameof(clientConfig));

            RegisterComponents(configurator.Registrar);

            var factory = new KafkaRegistrationRiderFactory<TContainerContext>(clientConfig, configure);
            configurator.SetRiderFactory(factory);
        }

        static void RegisterComponents(IContainerRegistrar registrar)
        {
            registrar.RegisterSingleInstance<IKafkaProducerProvider>(provider => provider.GetRequiredService<IKafkaRider>());
        }
    }
}
