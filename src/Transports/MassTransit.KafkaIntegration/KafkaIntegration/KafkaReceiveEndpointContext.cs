namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface KafkaReceiveEndpointContext<TKey, TValue> :
        ReceiveEndpointContext
        where TValue : class
    {
        IConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor { get; }
    }
}
