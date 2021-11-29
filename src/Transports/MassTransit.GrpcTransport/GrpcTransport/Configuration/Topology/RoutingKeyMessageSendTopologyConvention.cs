namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public class RoutingKeyMessageSendTopologyConvention<TMessage> :
        IRoutingKeyMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        IMessageRoutingKeyFormatter<TMessage> _formatter;

        public RoutingKeyMessageSendTopologyConvention(IRoutingKeyFormatter formatter)
        {
            if (formatter != null)
                SetFormatter(formatter);
        }

        bool IMessageSendTopologyConvention<TMessage>.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            if (_formatter != null)
            {
                messageSendTopology = new SetRoutingKeyMessageSendTopology<TMessage>(_formatter);
                return true;
            }

            messageSendTopology = null;
            return false;
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        public void SetFormatter(IRoutingKeyFormatter formatter)
        {
            _formatter = new MessageRoutingKeyFormatter<TMessage>(formatter);
        }

        public void SetFormatter(IMessageRoutingKeyFormatter<TMessage> formatter)
        {
            _formatter = formatter;
        }
    }
}
