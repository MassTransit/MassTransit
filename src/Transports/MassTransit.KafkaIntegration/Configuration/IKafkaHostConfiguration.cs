namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Specifications;
    using Transport;


    public interface IKafkaHostConfiguration :
        ISpecification
    {
        IReadOnlyDictionary<string, string> Configuration { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IKafkaRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
