namespace MassTransit.AzureServiceBusTransport
{
    public class MessageSessionIdFormatter<TMessage> :
        IMessageSessionIdFormatter<TMessage>
        where TMessage : class
    {
        readonly ISessionIdFormatter _formatter;

        public MessageSessionIdFormatter(ISessionIdFormatter formatter)
        {
            _formatter = formatter;
        }

        public string FormatSessionId(SendContext<TMessage> context)
        {
            return _formatter.FormatSessionId(context);
        }
    }
}
