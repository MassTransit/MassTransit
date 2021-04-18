namespace MassTransit.GrpcTransport.Topology
{
    using MassTransit.Topology;


    public interface IGrpcPublishTopology :
        IPublishTopology
    {
        new IGrpcMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
