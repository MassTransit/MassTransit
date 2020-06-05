namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public interface IKafkaProducerProvider
    {
        IKafkaProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext = default)
            where TValue : class;
    }
}
