namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Registration;
    using Registration;
    using Scoping;
    using Transport;


    public static class KafkaProducerRegistrationExtensions
    {
        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, string topicName,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<Null, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(topicName, _ => default, configure);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<Null, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(topicName, producerConfig, _ => default, configure);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));

            var registration = new KafkaProducerRegistrationConfigurator<TKey, T>(topicName, configure);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(topicName, provider));
            configurator.AddRegistration(registration);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var registration = new KafkaProducerRegistrationConfigurator<TKey, T>(topicName, configure, producerConfig);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(topicName, provider));
            configurator.AddRegistration(registration);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="keyResolver">Key resolver</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            KafkaKeyResolver<TKey, T> keyResolver,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(topicName, configure);
            configurator.Registrar.Register<ITopicProducer<T>>(provider =>
                new KeyedTopicProducer<TKey, T>(provider.GetRequiredService<ITopicProducer<TKey, T>>(), keyResolver));
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <param name="keyResolver">Key resolver</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig, KafkaKeyResolver<TKey, T> keyResolver,
            Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(topicName, producerConfig, configure);
            configurator.Registrar.Register<ITopicProducer<T>>(provider =>
                new KeyedTopicProducer<TKey, T>(provider.GetRequiredService<ITopicProducer<TKey, T>>(), keyResolver));
        }

        static ITopicProducer<TKey, T> GetProducer<TKey, T>(string topicName, IConfigurationServiceProvider provider)
            where T : class
        {
            var address = new Uri($"topic:{topicName}");

            return GetProducer<TKey, T>(address, provider);
        }

        static ITopicProducer<TKey, T> GetProducer<TKey, T>(Uri address, IConfigurationServiceProvider provider)
            where T : class
        {
            var rider = provider.GetRequiredService<IKafkaRider>();

            var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
            if (contextProvider != null)
            {
                return contextProvider.HasContext
                    ? rider.GetProducer<TKey, T>(address, contextProvider.GetContext())
                    : rider.GetProducer<TKey, T>(address);
            }

            return rider.GetProducer<TKey, T>(address, provider.GetService<ConsumeContext>());
        }
    }
}
