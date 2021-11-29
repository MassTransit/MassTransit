namespace MassTransit
{
    public interface IInMemoryPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IInMemoryPublishTopology
    {
        new IInMemoryMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
