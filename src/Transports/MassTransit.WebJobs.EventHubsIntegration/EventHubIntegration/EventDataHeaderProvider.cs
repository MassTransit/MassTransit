namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.EventHubs;
    using Logging;
    using Transports;


    public class EventDataHeaderProvider :
        IHeaderProvider
    {
        readonly EventData _eventData;

        public EventDataHeaderProvider(EventData eventData)
        {
            _eventData = eventData;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (!string.IsNullOrWhiteSpace(_eventData.MessageId))
                yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _eventData.MessageId);
            if (!string.IsNullOrWhiteSpace(_eventData.CorrelationId))
                yield return new KeyValuePair<string, object>(nameof(_eventData.CorrelationId), _eventData.CorrelationId);
            if (!string.IsNullOrWhiteSpace(_eventData.ContentType))
                yield return new KeyValuePair<string, object>(MessageHeaders.ContentType, _eventData.ContentType);

            if (_eventData.Properties != null)
            {
                foreach (KeyValuePair<string, object> header in _eventData.Properties)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_eventData.Properties != null)
            {
                if (_eventData.Properties.TryGetValue(key, out value))
                    return true;

                if (DiagnosticHeaders.ActivityId.Equals(key, StringComparison.OrdinalIgnoreCase)
                    && _eventData.Properties.TryGetValue(DiagnosticHeaders.DiagnosticId, out value))
                    return true;
            }

            if (nameof(_eventData.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _eventData.MessageId;
                return !string.IsNullOrWhiteSpace(_eventData.MessageId);
            }

            if (nameof(_eventData.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _eventData.CorrelationId;
                return !string.IsNullOrWhiteSpace(_eventData.CorrelationId);
            }

            if (MessageHeaders.ContentType.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _eventData.ContentType;
                return value != null;
            }

            value = default;
            return false;
        }
    }
}
