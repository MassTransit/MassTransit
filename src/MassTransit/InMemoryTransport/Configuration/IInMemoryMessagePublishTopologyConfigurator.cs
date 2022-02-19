namespace MassTransit
{
    using Transports.Fabric;


    public interface IInMemoryMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IInMemoryMessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopologyConfigurator
        where TMessage : class
    {
        new ExchangeType ExchangeType { set; }
    }


    public interface IInMemoryMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator
    {
    }
}
