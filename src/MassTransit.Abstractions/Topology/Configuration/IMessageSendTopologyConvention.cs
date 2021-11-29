namespace MassTransit.Configuration
{
    public interface IMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention
        where TMessage : class
    {
        bool TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology);
    }


    public interface IMessageSendTopologyConvention
    {
        bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class;
    }
}
