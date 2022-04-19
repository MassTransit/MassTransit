namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using DependencyInjection;
    using KafkaIntegration;
    using KafkaIntegration.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


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

            var registration = new KafkaProducerRegistration<TKey, T>(topicName, configure);
            configurator.TryAddScoped<IKafkaRider, ITopicProducer<TKey, T>>((rider, provider) => GetProducer<TKey, T>(topicName, rider, provider));
            configurator.Registrar.GetOrAdd<IKafkaProducerRegistration>(typeof(IKafkaProducerRegistration), _ => registration);
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

            var registration = new KafkaProducerRegistration<TKey, T>(topicName, configure, producerConfig);
            configurator.TryAddScoped<IKafkaRider, ITopicProducer<TKey, T>>((rider, provider) => GetProducer<TKey, T>(topicName, rider, provider));
            configurator.Registrar.GetOrAdd<IKafkaProducerRegistration>(typeof(IKafkaProducerRegistration), _ => registration);
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
            configurator.TryAddScoped<ITopicProducer<T>>(provider =>
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
            configurator.TryAddScoped<ITopicProducer<T>>(provider =>
                new KeyedTopicProducer<TKey, T>(provider.GetRequiredService<ITopicProducer<TKey, T>>(), keyResolver));
        }

        static ITopicProducer<TKey, T> GetProducer<TKey, T>(string topicName, IKafkaRider rider, IServiceProvider provider)
            where T : class
        {
            var address = new Uri($"topic:{topicName}");

            return GetProducer<TKey, T>(address, rider, provider);
        }

        static ITopicProducer<TKey, T> GetProducer<TKey, T>(Uri address, IKafkaRider rider, IServiceProvider provider)
            where T : class
        {
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
