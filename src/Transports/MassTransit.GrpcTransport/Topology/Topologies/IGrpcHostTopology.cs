namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using MassTransit.Topology;


    public interface IGrpcHostTopology :
        IHostTopology
    {
        new IGrpcMessagePublishTopology<T> Publish<T>()
            where T : class;
    }
}