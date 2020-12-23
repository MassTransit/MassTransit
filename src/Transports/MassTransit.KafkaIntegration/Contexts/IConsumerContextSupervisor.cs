namespace MassTransit.KafkaIntegration.Contexts
{
    using Transport;
    using Transports;


    public interface IConsumerContextSupervisor<TKey, TValue> :
        ITransportSupervisor<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
    }
}
