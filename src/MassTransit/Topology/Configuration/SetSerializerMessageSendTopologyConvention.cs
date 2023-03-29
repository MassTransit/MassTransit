namespace MassTransit.Configuration
{
    using System.Net.Mime;
    using Topology;


    public class SetSerializerMessageSendTopologyConvention<TMessage> :
        ISetSerializerMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        ContentType _contentType;

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        bool IMessageSendTopologyConvention<TMessage>.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            if (_contentType != null)
            {
                messageSendTopology = new SetSerializerMessageSendTopology<TMessage>(_contentType);
                return true;
            }

            messageSendTopology = null;
            return false;
        }

        public void SetSerializer(ContentType contentType)
        {
            _contentType = contentType;
        }
    }
}
