namespace MassTransit.EventHubIntegration
{
    using System.Threading.Tasks;
    using GreenPipes.Agents;
    using Transports.Metrics;


    public interface IEventHubDataReceiver :
        IAgent,
        DeliveryMetrics
    {
        Task Start();
    }
}
