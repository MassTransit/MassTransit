namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface TopicHandle :
        EntityHandle
    {
        Topic Topic { get; }
    }
}
