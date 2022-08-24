namespace MassTransit
{
    using System;


    public interface IInMemoryPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IInMemoryPublishTopology
    {
        new IInMemoryMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        new IInMemoryMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
    }
}
