namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


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
        ITopicConfigurator
    {
    }
}
