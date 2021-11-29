namespace MassTransit
{
    using AmazonSqsTransport.Topology;
    using Topology;


    public interface IAmazonSqsPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IAmazonSqsPublishTopology
    {
        new IAmazonSqsMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
