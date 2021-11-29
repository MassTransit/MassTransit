namespace MassTransit.AzureServiceBusTransport
{
    public interface IMessageSessionIdFormatter<in TMessage>
        where TMessage : class
    {
        string FormatSessionId(SendContext<TMessage> context);
    }
}
