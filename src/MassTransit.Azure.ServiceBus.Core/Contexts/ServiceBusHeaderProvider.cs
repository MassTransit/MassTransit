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
            yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _context.MessageId);
            yield return new KeyValuePair<string, object>(nameof(_context.CorrelationId), _context.CorrelationId);
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
                return true;
            }

            if (nameof(_context.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.CorrelationId;
                return true;
            }

            if ("Content-Type".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.ContentType.MediaType;
                return true;
            }

            value = default;
            return false;
        }
    }
}
