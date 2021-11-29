namespace MassTransit.KafkaIntegration
{
    using System;
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
        ITopicProducer<TKey, TValue> CreateProducer<TKey, TValue>(IBusInstance busInstance, Uri address)
            where TValue : class;
    }
}
