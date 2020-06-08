namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Registration;
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
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, string topicName)
            where T : class
        {
            configurator.Registrar.Register(provider => GetProducer<Null, T>(topicName, provider));
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="address">The topic address</param>
        /// <typeparam name="T">The message type</typeparam>
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, Uri address)
            where T : class
        {
            configurator.Registrar.Register(provider => GetProducer<Null, T>(address, provider));
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string topicName)
            where T : class
        {
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(topicName, provider));
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingKafka configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="address">The topic address</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, Uri address)
            where T : class
        {
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(address, provider));
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
            var producerProvider = provider.GetRequiredService<IKafkaProducerProvider>();

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
