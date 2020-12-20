namespace MassTransit.KafkaIntegration
{
    using Context;
    using Contexts;


    public interface IKafkaReceiveEndpointContext<TKey, TValue>:
        ReceiveEndpointContext
        where TValue : class
    {
        IKafkaConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor { get; }
    }
}
