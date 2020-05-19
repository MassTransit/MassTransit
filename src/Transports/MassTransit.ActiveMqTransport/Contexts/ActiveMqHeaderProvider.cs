namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Apache.NMS;
    using Context;


    public class ActiveMqHeaderProvider :
        IHeaderProvider
    {
        readonly IMessage _message;
        readonly ActiveMqHeaderAdapter _adapter;

        public ActiveMqHeaderProvider(IMessage message)
        {
            _message = message;
            _adapter = new ActiveMqHeaderAdapter(message.Properties);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(MessageHeaders.TransportMessageId, _message.NMSMessageId);
            yield return new KeyValuePair<string, object>(nameof(MessageContext.CorrelationId), _message.NMSCorrelationID);

            foreach (var header in _adapter.GetAll())
                yield return header;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (MessageHeaders.TransportMessageId.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.NMSMessageId;
                return true;
            }

            if (nameof(MessageContext.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.NMSCorrelationID;
                return true;
            }

            return _adapter.TryGetHeader(key, out value);
        }
    }
}
