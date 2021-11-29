namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading.Tasks;
    using Transports;


    public interface IReceiver :
        IAgent
    {
        DeliveryMetrics GetDeliveryMetrics();

        Task Start();
    }
}
