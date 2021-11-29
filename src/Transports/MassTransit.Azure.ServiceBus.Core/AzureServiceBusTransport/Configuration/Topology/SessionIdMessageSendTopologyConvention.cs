namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public class SessionIdMessageSendTopologyConvention<TMessage> :
        ISessionIdMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        IMessageSessionIdFormatter<TMessage> _formatter;

        public SessionIdMessageSendTopologyConvention(ISessionIdFormatter formatter)
        {
            if (formatter != null)
                SetFormatter(formatter);
        }

        public bool TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            if (_formatter != null)
            {
                messageSendTopology = new SetSessionIdMessageSendTopology<TMessage>(_formatter);
                return true;
            }

            messageSendTopology = null;
            return false;
        }

        public bool TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
            where T : class
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        public void SetFormatter(ISessionIdFormatter formatter)
        {
            _formatter = new MessageSessionIdFormatter<TMessage>(formatter);
        }

        public void SetFormatter(IMessageSessionIdFormatter<TMessage> formatter)
        {
            _formatter = formatter;
        }
    }
}
