namespace MassTransit
{
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
