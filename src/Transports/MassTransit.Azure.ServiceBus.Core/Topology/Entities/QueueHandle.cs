namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface QueueHandle :
        EntityHandle
    {
        Queue Queue { get; }
    }
}
