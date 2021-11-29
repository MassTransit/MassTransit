namespace MassTransit.AzureServiceBusTransport
{
    public class MessagePartitionKeyFormatter<TMessage> :
        IMessagePartitionKeyFormatter<TMessage>
        where TMessage : class
    {
        readonly IPartitionKeyFormatter _formatter;

        public MessagePartitionKeyFormatter(IPartitionKeyFormatter formatter)
        {
            _formatter = formatter;
        }

        public string FormatPartitionKey(SendContext<TMessage> context)
        {
            return _formatter.FormatPartitionKey(context);
        }
    }
}
