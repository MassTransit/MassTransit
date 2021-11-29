namespace MassTransit.Configuration
{
    public interface IMessagePublishTopologyConvention<TMessage> :
        IMessagePublishTopologyConvention
        where TMessage : class
    {
        bool TryGetMessagePublishTopology(out IMessagePublishTopology<TMessage> messagePublishTopology);
    }


    public interface IMessagePublishTopologyConvention
    {
        bool TryGetMessagePublishTopologyConvention<T>(out IMessagePublishTopologyConvention<T> convention)
            where T : class;
    }
}
