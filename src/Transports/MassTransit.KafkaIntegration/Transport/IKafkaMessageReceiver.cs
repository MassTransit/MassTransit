namespace MassTransit.KafkaIntegration.Transport
{
    using GreenPipes.Agents;
    using Transports.Metrics;


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
