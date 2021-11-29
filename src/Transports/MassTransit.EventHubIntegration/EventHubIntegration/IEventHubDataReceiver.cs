namespace MassTransit.EventHubIntegration
{
    using System.Threading.Tasks;
    using Transports;


    public interface IEventHubDataReceiver :
        IAgent,
        DeliveryMetrics
    {
        Task Start();
    }
}
