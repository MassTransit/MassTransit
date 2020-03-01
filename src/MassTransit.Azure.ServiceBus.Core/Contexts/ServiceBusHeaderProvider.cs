namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using Context;


    public class ServiceBusHeaderProvider :
        IHeaderProvider
    {
        readonly ServiceBusReceiveContext _context;

        public ServiceBusHeaderProvider(ServiceBusReceiveContext context)
        {
            _context = context;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (!string.IsNullOrWhiteSpace(_context.MessageId))
                yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _context.MessageId);
            if (!string.IsNullOrWhiteSpace(_context.CorrelationId))
                yield return new KeyValuePair<string, object>(nameof(_context.CorrelationId), _context.CorrelationId);
            if (_context.ContentType != null)
                yield return new KeyValuePair<string, object>("Content-Type", _context.ContentType);

            if (_context.Properties != null)
            {
                foreach (KeyValuePair<string, object> header in _context.Properties)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_context.Properties != null && _context.Properties.TryGetValue(key, out value))
            {
                return true;
            }

            if (nameof(_context.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.MessageId;
                return !string.IsNullOrWhiteSpace(_context.MessageId);
            }

            if (nameof(_context.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.CorrelationId;
                return !string.IsNullOrWhiteSpace(_context.CorrelationId);
            }

            if ("Content-Type".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.ContentType?.MediaType;
                return value != null;
            }

            value = default;
            return false;
        }
    }
}
