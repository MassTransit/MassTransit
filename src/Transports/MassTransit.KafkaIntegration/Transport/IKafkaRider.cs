namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using Riders;


    public interface IKafkaRider :
        IRiderControl
    {
        ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext = default)
            where TValue : class;
    }
}
