namespace MassTransit
{
    using Transports.Fabric;


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
