namespace MassTransit
{
    using System;
    using Transports;


    public interface IKafkaRider :
        IRiderControl,
        IKafkaTopicEndpointConnector
    {
        ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext = default)
            where TValue : class;
    }
}
