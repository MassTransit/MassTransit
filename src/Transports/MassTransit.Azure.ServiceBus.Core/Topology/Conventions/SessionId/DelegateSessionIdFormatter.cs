namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions.SessionId
{
    using System;


    public class DelegateSessionIdFormatter<TMessage> :
        IMessageSessionIdFormatter<TMessage>
        where TMessage : class
    {
        readonly Func<SendContext<TMessage>, string> _formatter;

        public DelegateSessionIdFormatter(Func<SendContext<TMessage>, string> formatter)
        {
            _formatter = formatter;
        }

        public string FormatSessionId(SendContext<TMessage> context)
        {
            return _formatter(context) ?? "";
        }
    }
}
