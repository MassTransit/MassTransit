namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface ConsumerHandle :
        EntityHandle
    {
        Consumer Consumer { get; }
    }
}
