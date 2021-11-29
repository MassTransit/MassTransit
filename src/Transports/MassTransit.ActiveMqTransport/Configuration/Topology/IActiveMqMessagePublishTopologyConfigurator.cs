namespace MassTransit
{
    using ActiveMqTransport.Topology;


    public interface IActiveMqMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IActiveMqMessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IActiveMqMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        IActiveMqMessagePublishTopology,
        IActiveMqTopicConfigurator
    {
    }
}
