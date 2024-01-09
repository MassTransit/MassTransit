namespace MassTransit
{
    using System;


    public interface ISqlPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        ISqlPublishTopology
    {
        new ISqlMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        new ISqlMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
    }
}
