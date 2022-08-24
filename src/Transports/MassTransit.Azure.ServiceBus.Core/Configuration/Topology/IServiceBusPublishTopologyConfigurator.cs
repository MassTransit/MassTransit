namespace MassTransit
{
    using System;


    public interface IServiceBusPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IServiceBusPublishTopology
    {
        new IServiceBusMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        new IServiceBusMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
    }
}
