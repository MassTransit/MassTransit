namespace MassTransit
{
    using System;
    using Confluent.Kafka;


    public static class TopicEndpointConnectorExtensions
    {
        /// <summary>
        /// Connect topic endpoint
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static HostReceiveEndpointHandle ConnectTopicEndpoint<T>(this IKafkaTopicEndpointConnector connector, string topicName, string groupId,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            return connector.ConnectTopicEndpoint(topicName, groupId, configure);
        }

        /// <summary>
        /// Connect topic endpoint
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="topicName">The topic name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static HostReceiveEndpointHandle ConnectTopicEndpoint<T>(this IKafkaTopicEndpointConnector connector, string topicName,
            ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            return connector.ConnectTopicEndpoint(topicName, consumerConfig, configure);
        }
    }
}
