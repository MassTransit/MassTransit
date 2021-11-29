namespace MassTransit
{
    using AmazonSqsTransport.Topology;
    using Topology;


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
