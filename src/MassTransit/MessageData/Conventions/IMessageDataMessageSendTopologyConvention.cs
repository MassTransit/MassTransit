namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;
    using Topology;


    public interface IMessageDataMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
