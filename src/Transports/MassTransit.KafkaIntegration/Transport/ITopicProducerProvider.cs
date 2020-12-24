namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public interface ITopicProducerProvider
    {
        ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class;
    }
}
