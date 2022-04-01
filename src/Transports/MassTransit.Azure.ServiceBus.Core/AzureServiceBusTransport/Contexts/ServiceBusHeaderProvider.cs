namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus;
    using Logging;
    using Transports;


    public class ServiceBusHeaderProvider :
        IHeaderProvider
    {
        readonly ServiceBusReceivedMessage _message;

        public ServiceBusHeaderProvider(ServiceBusReceivedMessage message)
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

            if (_message.ApplicationProperties != null)
            {
                foreach (KeyValuePair<string, object> header in _message.ApplicationProperties)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_message.ApplicationProperties != null)
            {
                if (_message.ApplicationProperties.TryGetValue(key, out value))
                    return true;

                if (DiagnosticHeaders.ActivityId.Equals(key, StringComparison.OrdinalIgnoreCase)
                    && _message.ApplicationProperties.TryGetValue(DiagnosticHeaders.DiagnosticId, out value))
                    return true;
            }

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
