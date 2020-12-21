namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public interface IKafkaProducerFactory
    {
        Uri TopicAddress { get; }
    }


    public interface IKafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory
        where TValue : class
    {
        ITopicProducer<TKey, TValue> CreateProducer(ConsumeContext consumeContext = null);
    }
}
