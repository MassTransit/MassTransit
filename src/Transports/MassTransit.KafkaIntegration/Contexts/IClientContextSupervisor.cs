namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using MassTransit.Registration;
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
        ITopicProducer<TKey, TValue> CreateProducer<TKey, TValue>(IBusInstance busInstance, Uri address)
            where TValue : class;
    }
}
