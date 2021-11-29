namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IConsumerContextSupervisor<TKey, TValue> :
        ITransportSupervisor<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
    }
}
