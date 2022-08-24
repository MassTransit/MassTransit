namespace MassTransit
{
    using System;


    public interface IGrpcPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IGrpcPublishTopology
    {
        new IGrpcMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        new IGrpcMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
    }
}
