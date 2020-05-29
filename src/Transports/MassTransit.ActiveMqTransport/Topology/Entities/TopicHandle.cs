namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface TopicHandle :
        EntityHandle
    {
        Topic Topic { get; }
    }
}
