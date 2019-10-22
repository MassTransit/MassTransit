namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading.Tasks;
    using GreenPipes.Agents;
    using Transports.Metrics;


    public interface IReceiver :
        IAgent
    {
        DeliveryMetrics GetDeliveryMetrics();

        Task Start();
    }
}
