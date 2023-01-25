namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;


    public interface IMessageDataMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
