namespace MassTransit
{
    using AmazonSqsTransport;
    using AmazonSqsTransport.Topology;
    using Topology;


    public interface IAmazonSqsMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IAmazonSqsMessagePublishTopology<TMessage>,
        IAmazonSqsMessagePublishTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IAmazonSqsMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        IAmazonSqsMessagePublishTopology,
        IAmazonSqsTopicConfigurator
    {
    }
}
