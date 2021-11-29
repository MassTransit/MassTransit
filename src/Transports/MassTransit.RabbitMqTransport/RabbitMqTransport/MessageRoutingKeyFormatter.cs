namespace MassTransit.RabbitMqTransport
{
    public class MessageRoutingKeyFormatter<TMessage> :
        IMessageRoutingKeyFormatter<TMessage>
        where TMessage : class
    {
        readonly IRoutingKeyFormatter _formatter;

        public MessageRoutingKeyFormatter(IRoutingKeyFormatter formatter)
        {
            _formatter = formatter;
        }

        public string FormatRoutingKey(RabbitMqSendContext<TMessage> context)
        {
            return _formatter.FormatRoutingKey(context);
        }
    }
}
