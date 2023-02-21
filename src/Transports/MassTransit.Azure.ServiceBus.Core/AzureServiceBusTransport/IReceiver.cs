namespace MassTransit.AzureServiceBusTransport
{
    using Transports;


    public interface IReceiver :
        IAgent,
        DeliveryMetrics
    {
        void Start();
    }
}
