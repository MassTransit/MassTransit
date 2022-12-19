namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IKafkaMessageReceiver :
        IAgent,
        DeliveryMetrics
    {
    }


    public interface IKafkaMessageConsumer<TKey, TValue> :
        IKafkaMessageReceiver
        where TValue : class
    {

    }
}
