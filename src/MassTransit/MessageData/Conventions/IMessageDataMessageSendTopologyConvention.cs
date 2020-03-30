namespace MassTransit.MessageData.Conventions
{
    using Topology;


    public interface IMessageDataMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
