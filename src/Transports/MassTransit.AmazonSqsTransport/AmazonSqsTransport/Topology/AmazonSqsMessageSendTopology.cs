namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public class AmazonSqsMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IAmazonSqsMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
