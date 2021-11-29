namespace MassTransit
{
    public interface IServiceBusPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IServiceBusPublishTopology
    {
        new IServiceBusMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
