namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface IAmazonSqsMessageSendTopologyConfigurator<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>,
        IAmazonSqsMessageSendTopology<TMessage>,
        IAmazonSqsMessageSendTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IAmazonSqsMessageSendTopologyConfigurator :
        IMessageSendTopologyConfigurator
    {
    }
}
