namespace MassTransit
{
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
