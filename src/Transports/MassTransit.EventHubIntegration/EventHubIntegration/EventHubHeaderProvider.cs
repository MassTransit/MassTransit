namespace MassTransit.EventHubIntegration;

using System;
using System.Collections.Generic;
using Azure.Messaging.EventHubs;
using Transports;


public class EventHubHeaderProvider :
    IHeaderProvider
{
    readonly EventData _eventData;

    public EventHubHeaderProvider(EventData eventData)
    {
        _eventData = eventData;
    }

    public IEnumerable<KeyValuePair<string, object>> GetAll()
    {
        if (!string.IsNullOrWhiteSpace(_eventData.MessageId))
            yield return new KeyValuePair<string, object>(nameof(MessageContext.MessageId), _eventData.MessageId);
        if (!string.IsNullOrWhiteSpace(_eventData.CorrelationId))
            yield return new KeyValuePair<string, object>(nameof(MessageContext.CorrelationId), _eventData.CorrelationId);

        foreach (var key in _eventData.Properties.Keys)
        {
            var value = _eventData.Properties[key];

            yield return new KeyValuePair<string, object>(key, value);
        }
    }

    public bool TryGetHeader(string key, out object value)
    {
        if (nameof(MessageContext.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
        {
            value = _eventData.MessageId;
            return value != null;
        }

        if (nameof(MessageContext.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
        {
            value = _eventData.CorrelationId;
            return value != null;
        }

        var found = _eventData.Properties.TryGetValue(key, out value);
        if (found)
            return true;

        value = null;
        return false;
    }
}
