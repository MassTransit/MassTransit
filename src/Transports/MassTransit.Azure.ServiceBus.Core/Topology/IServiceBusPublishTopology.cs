namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;


    public interface IServiceBusPublishTopology :
        IPublishTopology
    {
        new IServiceBusMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
