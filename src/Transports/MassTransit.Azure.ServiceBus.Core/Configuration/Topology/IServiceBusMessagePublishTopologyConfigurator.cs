namespace MassTransit
{
    public interface IServiceBusMessagePublishTopologyConfigurator<TMessage> :
        IServiceBusMessagePublishTopologyConfigurator,
        IMessagePublishTopologyConfigurator<TMessage>,
        IServiceBusMessagePublishTopology<TMessage>
        where TMessage : class
    {
    }


    public interface IServiceBusMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        IServiceBusTopicConfigurator
    {
    }
}
