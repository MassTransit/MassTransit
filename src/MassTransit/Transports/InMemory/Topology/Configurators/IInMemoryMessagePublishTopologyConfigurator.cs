namespace MassTransit.Transports.InMemory.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IInMemoryMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IInMemoryMessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IInMemoryMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator
    {
    }
}
