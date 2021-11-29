namespace MassTransit.KafkaIntegration
{
    using System;


    public interface ITopicProducerProvider
    {
        ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class;
    }
}
