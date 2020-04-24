namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface IAmazonSqsPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IAmazonSqsPublishTopology
    {
        new IAmazonSqsMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
