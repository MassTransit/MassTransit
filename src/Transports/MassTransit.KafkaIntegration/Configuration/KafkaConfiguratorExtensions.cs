namespace MassTransit
{
    using System;
    using Confluent.Kafka;


    public static class KafkaConfiguratorExtensions
    {
        /// <summary>
        /// Configure Kafka host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="server"></param>
        /// <param name="configure"></param>
        public static void Host(this IKafkaFactoryConfigurator configurator, string server, Action<IKafkaHostConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            configurator.Host(new[] { server }, configure);
        }

        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static void TopicEndpoint<T>(this IKafkaFactoryConfigurator configurator, string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.TopicEndpoint(topicName, groupId, configure);
        }

        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static void TopicEndpoint<T>(this IKafkaFactoryConfigurator configurator, string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.TopicEndpoint(topicName, consumerConfig, configure);
        }

        /// <summary>
        /// Configure kafka topic producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Value key type</typeparam>
        internal static void TopicProducer<T>(this IKafkaFactoryConfigurator configurator, string topicName,
            Action<IKafkaProducerConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.TopicProducer(topicName, configure);
        }

        /// <summary>
        /// Configure kafka topic producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig">Producer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal static void TopicProducer<TValue>(this IKafkaFactoryConfigurator configurator, string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<Ignore, TValue>> configure)
            where TValue : class
        {
            configurator.TopicProducer(topicName, producerConfig, configure);
        }

        /// <summary>
        /// Configure kafka topic producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="producerConfig">Producer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal static void TopicProducer<TKey, TValue>(this IKafkaFactoryConfigurator configurator, string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            configurator.TopicProducer(topicName, producerConfig, configure);
        }
    }
}
