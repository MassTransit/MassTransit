namespace MassTransit.KafkaIntegration
{
    using Context;
    using Contexts;


    public interface IKafkaReceiveEndpointContext<TKey, TValue> :
        ReceiveEndpointContext
        where TValue : class
    {
        IConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor { get; }
    }
}
