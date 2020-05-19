namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;


    public interface IServiceBusMessagePublishTopologyConfigurator<TMessage> :
        IServiceBusMessagePublishTopologyConfigurator,
        IMessagePublishTopologyConfigurator<TMessage>,
        IServiceBusMessagePublishTopology<TMessage>
        where TMessage : class
    {
    }


    public interface IServiceBusMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        ITopicConfigurator
    {
    }
}
