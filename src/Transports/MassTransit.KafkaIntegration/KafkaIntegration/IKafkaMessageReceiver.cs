namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IKafkaMessageReceiver :
        IAgent,
        DeliveryMetrics
    {
    }


    public interface IKafkaMessageReceiver<TKey, TValue> :
        IKafkaMessageReceiver
        where TValue : class
    {
    }
}
