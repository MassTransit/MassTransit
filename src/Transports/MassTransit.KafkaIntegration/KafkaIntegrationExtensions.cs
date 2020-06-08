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
        public static void UsingKafka(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            RegisterComponents(configurator.Registrar);

            var factory = new KafkaRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);
        }

        public static void UsingKafka(this IRiderRegistrationConfigurator configurator,
            ClientConfig clientConfig,
            Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (clientConfig == null)
                throw new ArgumentNullException(nameof(clientConfig));

            RegisterComponents(configurator.Registrar);

            var factory = new KafkaRegistrationRiderFactory(clientConfig, configure);
            configurator.SetRiderFactory(factory);
        }

        static void RegisterComponents(IContainerRegistrar registrar)
        {
            registrar.RegisterSingleInstance<IKafkaProducerProvider>(provider => provider.GetRequiredService<IKafkaRider>());
        }
    }
}
