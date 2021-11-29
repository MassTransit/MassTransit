namespace MassTransit
{
    using GrpcTransport.Topology;


    public interface IGrpcPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IGrpcPublishTopology
    {
        new IGrpcMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}