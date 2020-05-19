namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;


    public interface IServiceBusPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IServiceBusPublishTopology
    {
        new IServiceBusMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
