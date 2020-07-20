namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Microsoft.Azure.ServiceBus;


    public class ServiceBusHeaderProvider :
        IHeaderProvider
    {
        readonly Message _message;

        public ServiceBusHeaderProvider(Message message)
        {
            _message = message;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (!string.IsNullOrWhiteSpace(_message.MessageId))
                yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _message.MessageId);
            if (!string.IsNullOrWhiteSpace(_message.CorrelationId))
                yield return new KeyValuePair<string, object>(nameof(_message.CorrelationId), _message.CorrelationId);
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                yield return new KeyValuePair<string, object>(MessageHeaders.ContentType, _message.ContentType);

            if (_message.UserProperties != null)
            {
                foreach (KeyValuePair<string, object> header in _message.UserProperties)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_message.UserProperties != null && _message.UserProperties.TryGetValue(key, out value))
                return true;

            if (nameof(_message.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.MessageId;
                return !string.IsNullOrWhiteSpace(_message.MessageId);
            }

            if (nameof(_message.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.CorrelationId;
                return !string.IsNullOrWhiteSpace(_message.CorrelationId);
            }

            if (MessageHeaders.ContentType.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.ContentType;
                return value != null;
            }

            value = default;
            return false;
        }
    }
}
