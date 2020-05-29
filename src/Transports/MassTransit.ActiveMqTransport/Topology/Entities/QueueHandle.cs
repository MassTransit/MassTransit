namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface QueueHandle :
        EntityHandle
    {
        Queue Queue { get; }
    }
}
