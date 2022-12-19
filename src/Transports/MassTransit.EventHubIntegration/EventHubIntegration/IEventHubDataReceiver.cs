namespace MassTransit.EventHubIntegration
{
    using Transports;


    public interface IEventHubDataReceiver :
        IAgent,
        DeliveryMetrics
    {
    }
}
