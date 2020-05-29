namespace MassTransit.AmazonSqsTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface TopicHandle :
        EntityHandle
    {
        Topic Topic { get; }
    }
}
