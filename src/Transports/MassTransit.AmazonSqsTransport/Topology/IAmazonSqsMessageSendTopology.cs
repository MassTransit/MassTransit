namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface IAmazonSqsMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
