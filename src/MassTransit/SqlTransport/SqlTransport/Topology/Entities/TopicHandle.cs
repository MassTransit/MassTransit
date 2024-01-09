namespace MassTransit.SqlTransport.Topology
{
    using MassTransit.Topology;


    public interface TopicHandle :
        EntityHandle
    {
        Topic Topic { get; }
    }
}
