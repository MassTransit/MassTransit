namespace MassTransit.GrpcTransport.Topology.Configurators
{
    using Contracts;
    using MassTransit.Topology;


    public interface IGrpcMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IGrpcMessagePublishTopology<TMessage>,
        IGrpcMessagePublishTopologyConfigurator
        where TMessage : class
    {
        new ExchangeType ExchangeType { set; }
    }


    public interface IGrpcMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator
    {
    }
}
