using System.Threading.Tasks;
using GreenPipes.Agents;
using MassTransit.Transports.Metrics;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbDataReceiver :
        IAgent,
        DeliveryMetrics
    {
        Task Start();
    }
}
