namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using Transports;


    public interface IKafkaProducerSpecification :
        ISpecification
    {
        string TopicName { get; }
    }


    public interface IKafkaProducerSpecification<TKey, TValue> :
        IKafkaProducerSpecification
        where TValue : class
    {
        KafkaSendTransportContext<TKey, TValue> CreateSendTransportContext(IBusInstance busInstance, Action onStop = null);
    }
}
