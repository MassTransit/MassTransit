namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface IAmazonSqsPublishTopology :
        IPublishTopology
    {
        new IAmazonSqsMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
