namespace MassTransit.GrpcTransport.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IGrpcMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IGrpcMessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IInMemoryMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator
    {
    }
}
