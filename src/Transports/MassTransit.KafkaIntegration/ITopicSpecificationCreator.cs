namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Specifications;


    public interface IConsumerSpecificationCreator
    {
        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;
    }
}
