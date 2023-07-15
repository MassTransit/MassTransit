namespace MassTransit.Transports
{
    using System;


    public class DelegateRoutingKeyFormatter<TMessage> :
        IMessageRoutingKeyFormatter<TMessage>
        where TMessage : class
    {
        readonly Func<SendContext<TMessage>, string> _formatter;

        public DelegateRoutingKeyFormatter(Func<SendContext<TMessage>, string> formatter)
        {
            _formatter = formatter;
        }

        public string FormatRoutingKey(SendContext<TMessage> context)
        {
            return _formatter(context);
        }
    }
}
