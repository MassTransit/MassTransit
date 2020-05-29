namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using MassTransit.Topology.Topologies;


    public class AmazonSqsMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IAmazonSqsMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
