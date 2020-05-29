namespace MassTransit.KafkaIntegration
{
    using System;
    using Configuration;
    using Confluent.Kafka;
    using Subscriptions;


    public static class KafkaConfiguratorExtensions
    {
        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topic">Topic name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        public static void Topic<TKey, TValue>(this IKafkaFactoryConfigurator configurator, string topic, string groupId,
            Action<IKafkaTopicConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            configurator.Topic(new ConstantTopicNameFormatter(topic), groupId, configure);
        }

        /// <summary>
        /// Subscribe to kafka topic
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="topic">Topic name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        public static void Topic<TKey, TValue>(this IKafkaFactoryConfigurator configurator, string topic, ConsumerConfig consumerConfig,
            Action<IKafkaTopicConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            configurator.Topic(new ConstantTopicNameFormatter(topic), consumerConfig, configure);
        }

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
            configurator.Host(new[] {server}, configure);
        }


        class ConstantTopicNameFormatter :
            ITopicNameFormatter
        {
            readonly string _topic;

            public ConstantTopicNameFormatter(string topic)
            {
                _topic = topic;
            }

            public string GetTopicName<TKey, TValue>()
                where TValue : class
            {
                return _topic;
            }
        }
    }
}
