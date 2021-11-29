namespace MassTransit.GrpcTransport
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

        public string FormatRoutingKey(GrpcSendContext<TMessage> context)
        {
            return _formatter.FormatRoutingKey(context);
        }
    }
}
