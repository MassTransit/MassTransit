namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using System;
    using System.Collections.Generic;
    using Transports;


    public class ActiveMqHeaderProvider :
        IHeaderProvider
    {
        readonly IMessage _message;

        public ActiveMqHeaderProvider(IMessage message)
        {
            _message = message;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(MessageHeaders.TransportMessageId, _message.NMSMessageId);
            yield return new KeyValuePair<string, object>(nameof(MessageContext.CorrelationId), _message.NMSCorrelationID);
            yield return new KeyValuePair<string, object>(nameof(MessageContext.ResponseAddress), GetResponseAddress(_message.NMSReplyTo));

            foreach (string key in _message.Properties.Keys)
            {
                var value = _message.Properties[key];

                yield return new KeyValuePair<string, object>(key, value);
            }
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

            if (nameof(MessageContext.ResponseAddress).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = GetResponseAddress(_message.NMSReplyTo);
                return true;
            }

            var found = _message.Properties.Contains(key);
            if (found)
            {
                value = _message.Properties[key];
                return true;
            }

            value = null;
            return false;
        }

        private static Uri GetResponseAddress(IDestination replyTo)
        {
            return replyTo switch
            {
                null => null,
                IQueue queue => new Uri($"queue:{queue.QueueName}"),
                ITopic topic => new Uri($"topic:{topic.TopicName}"),
                _ => null
            };
        }
    }
}
