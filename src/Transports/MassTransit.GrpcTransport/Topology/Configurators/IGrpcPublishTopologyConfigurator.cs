namespace MassTransit.GrpcTransport.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IGrpcPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IGrpcPublishTopology
    {
        new IGrpcMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}