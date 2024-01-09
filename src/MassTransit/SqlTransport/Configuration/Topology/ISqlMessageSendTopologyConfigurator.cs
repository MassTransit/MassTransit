namespace MassTransit
{
    public interface ISqlMessageSendTopologyConfigurator<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>,
        ISqlMessageSendTopology<TMessage>,
        ISqlMessageSendTopologyConfigurator
        where TMessage : class
    {
    }


    public interface ISqlMessageSendTopologyConfigurator :
        IMessageSendTopologyConfigurator
    {
    }
}
