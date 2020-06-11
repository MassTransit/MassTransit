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
        /// <typeparam name="T">The message type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<Ignore, T> AddProducer<T>(this IRiderRegistrationConfigurator configurator, string topicName)
            where T : class
        {
            return configurator.AddProducer<Ignore, T>(topicName, _ => default);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <typeparam name="T">The message type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<Ignore, T> AddProducer<T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig)
            where T : class
        {
            return configurator.AddProducer<Ignore, T>(topicName, producerConfig, _ => default);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<TKey, T> AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));

            var registration = new KafkaProducerRegistrationConfigurator<TKey, T>(topicName);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(topicName, provider));
            configurator.AddRegistration(registration);
            return registration;
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<TKey, T> AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var registration = new KafkaProducerRegistrationConfigurator<TKey, T>(topicName, producerConfig);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(topicName, provider));
            configurator.AddRegistration(registration);
            return registration;
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="keyResolver">Key resolver</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<TKey, T> AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            KafkaKeyResolver<TKey, T> keyResolver)
            where T : class
        {
            IKafkaProducerRegistrationConfigurator<TKey, T> registration = configurator.AddProducer<TKey, T>(topicName);
            configurator.Registrar.Register<IKafkaProducer<T>>(provider =>
                new KeyedKafkaProducer<TKey, T>(provider.GetRequiredService<IKafkaProducer<TKey, T>>(), keyResolver));

            return registration;
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig"></param>
        /// <param name="keyResolver">Key resolver</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static IKafkaProducerRegistrationConfigurator<TKey, T> AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName,
            ProducerConfig producerConfig, KafkaKeyResolver<TKey, T> keyResolver)
            where T : class
        {
            IKafkaProducerRegistrationConfigurator<TKey, T> registration = configurator.AddProducer<TKey, T>(topicName, producerConfig);
            configurator.Registrar.Register<IKafkaProducer<T>>(provider =>
                new KeyedKafkaProducer<TKey, T>(provider.GetRequiredService<IKafkaProducer<TKey, T>>(), keyResolver));

            return registration;
        }

        static IKafkaProducer<TKey, T> GetProducer<TKey, T>(string topicName, IConfigurationServiceProvider provider)
            where T : class
        {
            var address = new Uri($"topic:{topicName}");

            return GetProducer<TKey, T>(address, provider);
        }

        static IKafkaProducer<TKey, T> GetProducer<TKey, T>(Uri address, IConfigurationServiceProvider provider)
            where T : class
        {
            IKafkaProducerProvider producerProvider = provider.GetRequiredService<IKafkaRider>();

            var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
            if (contextProvider != null)
            {
                return contextProvider.HasContext
                    ? producerProvider.GetProducer<TKey, T>(address, contextProvider.GetContext())
                    : producerProvider.GetProducer<TKey, T>(address);
            }

            return producerProvider.GetProducer<TKey, T>(address, provider.GetService<ConsumeContext>());
        }
    }
}
