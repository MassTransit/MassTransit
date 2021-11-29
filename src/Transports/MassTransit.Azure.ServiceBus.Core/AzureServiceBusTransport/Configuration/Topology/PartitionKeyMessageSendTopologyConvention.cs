namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public class PartitionKeyMessageSendTopologyConvention<TMessage> :
        IPartitionKeyMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        IMessagePartitionKeyFormatter<TMessage> _formatter;

        public PartitionKeyMessageSendTopologyConvention(IPartitionKeyFormatter formatter)
        {
            if (formatter != null)
                SetFormatter(formatter);
        }

        public bool TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            if (_formatter != null)
            {
                messageSendTopology = new SetPartitionKeyMessageSendTopology<TMessage>(_formatter);
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

        public void SetFormatter(IPartitionKeyFormatter formatter)
        {
            _formatter = new MessagePartitionKeyFormatter<TMessage>(formatter);
        }

        public void SetFormatter(IMessagePartitionKeyFormatter<TMessage> formatter)
        {
            _formatter = formatter;
        }
    }
}
