namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaIntegration;
    using KafkaIntegration.Configuration;
    using Transports;


    public interface IKafkaHostConfiguration :
        ISpecification
    {
        IReadOnlyDictionary<string, string> Configuration { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        KafkaSendTransportContext<TKey, TValue> CreateSendTransportContext<TKey, TValue>(IBusInstance busInstance, string topic)
            where TValue : class;

        IKafkaProducerSpecification CreateSpecification<TKey, TValue>(string topicName, Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaProducerSpecification CreateSpecification<TKey, TValue>(string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
