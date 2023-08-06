namespace MassTransit
{
    using System;


    public interface ITopicProducerProvider :
        ISendObserverConnector
    {
        ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class;
    }
}
