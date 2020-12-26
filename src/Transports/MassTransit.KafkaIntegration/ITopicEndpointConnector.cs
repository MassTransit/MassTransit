namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Registration;


    public interface ITopicEndpointConnector
    {
        HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, string groupId,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;
    }
}
