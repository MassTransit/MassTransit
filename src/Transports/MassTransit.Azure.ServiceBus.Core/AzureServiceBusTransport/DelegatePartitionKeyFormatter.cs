namespace MassTransit.AzureServiceBusTransport
{
    using System;


    public class DelegatePartitionKeyFormatter<TMessage> :
        IMessagePartitionKeyFormatter<TMessage>
        where TMessage : class
    {
        readonly Func<SendContext<TMessage>, string> _formatter;

        public DelegatePartitionKeyFormatter(Func<SendContext<TMessage>, string> formatter)
        {
            _formatter = formatter;
        }

        public string FormatPartitionKey(SendContext<TMessage> context)
        {
            return _formatter(context) ?? "";
        }
    }
}
