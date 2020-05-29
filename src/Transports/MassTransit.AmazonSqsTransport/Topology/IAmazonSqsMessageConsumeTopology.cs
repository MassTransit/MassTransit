namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface IAmazonSqsMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
