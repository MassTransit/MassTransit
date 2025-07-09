namespace MassTransit;

using System;


public interface IAmazonSqsPublishTopologyConfigurator :
    IPublishTopologyConfigurator,
    IAmazonSqsPublishTopology
{
    new IAmazonSqsMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
        where T : class;

    new IAmazonSqsMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
}
