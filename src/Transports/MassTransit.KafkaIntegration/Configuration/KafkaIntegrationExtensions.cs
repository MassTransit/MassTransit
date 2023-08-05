namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using DependencyInjection;
    using KafkaIntegration;
    using KafkaIntegration.Configuration;


    public static class KafkaIntegrationExtensions
    {
        public static void UsingKafka(this IRiderRegistrationConfigurator configurator, Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaRegistrationRiderFactory(configure);

            configurator.SetRiderFactory(factory);
            configurator.TryAddScoped<IKafkaRider, ITopicProducerProvider>((rider, provider) => rider.GetScopedTopicProducerProvider(provider));
        }

        public static void UsingKafka(this IRiderRegistrationConfigurator configurator, ClientConfig clientConfig,
            Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (clientConfig == null)
                throw new ArgumentNullException(nameof(clientConfig));

            var factory = new KafkaRegistrationRiderFactory(clientConfig, configure);
            configurator.SetRiderFactory(factory);
            configurator.TryAddScoped<IKafkaRider, ITopicProducerProvider>((rider, provider) => rider.GetScopedTopicProducerProvider(provider));
        }

        public static void UsingKafka<TBus>(this IRiderRegistrationConfigurator<TBus> configurator,
            Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
            where TBus : class, IBus
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new KafkaRegistrationRiderFactory(configure);

            configurator.SetRiderFactory(factory);
            configurator.TryAddScoped<IKafkaRider, Bind<TBus, ITopicProducerProvider>>((rider, provider) =>
                Bind<TBus>.Create(rider.GetScopedTopicProducerProvider(provider)));
        }

        public static void UsingKafka<TBus>(this IRiderRegistrationConfigurator<TBus> configurator, ClientConfig clientConfig,
            Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
            where TBus : class, IBus
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (clientConfig == null)
                throw new ArgumentNullException(nameof(clientConfig));

            var factory = new KafkaRegistrationRiderFactory(clientConfig, configure);

            configurator.SetRiderFactory(factory);
            configurator.TryAddScoped<IKafkaRider, Bind<TBus, ITopicProducerProvider>>((rider, provider) =>
                Bind<TBus>.Create(rider.GetScopedTopicProducerProvider(provider)));
        }
    }
}
