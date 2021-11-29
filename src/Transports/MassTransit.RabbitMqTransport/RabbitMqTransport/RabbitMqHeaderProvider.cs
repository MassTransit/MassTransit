namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Transports;


    public class RabbitMqHeaderProvider :
        IHeaderProvider
    {
        readonly RabbitMqBasicConsumeContext _context;

        public RabbitMqHeaderProvider(RabbitMqBasicConsumeContext context)
        {
            _context = context;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (!string.IsNullOrWhiteSpace(_context.Exchange))
                yield return new KeyValuePair<string, object>(RabbitMqHeaders.Exchange, _context.Exchange);
            if (!string.IsNullOrWhiteSpace(_context.RoutingKey))
                yield return new KeyValuePair<string, object>(RabbitMqHeaders.RoutingKey, _context.RoutingKey);
            yield return new KeyValuePair<string, object>(RabbitMqHeaders.DeliveryTag, _context.DeliveryTag);
            if (!string.IsNullOrWhiteSpace(_context.ConsumerTag))
                yield return new KeyValuePair<string, object>(RabbitMqHeaders.ConsumerTag, _context.ConsumerTag);
            if (!string.IsNullOrWhiteSpace(_context.Properties.MessageId))
                yield return new KeyValuePair<string, object>(nameof(MessageHeaders.MessageId), _context.Properties.MessageId);
            if (!string.IsNullOrWhiteSpace(_context.Properties.CorrelationId))
                yield return new KeyValuePair<string, object>(nameof(_context.Properties.CorrelationId), _context.Properties.CorrelationId);

            if (_context.Properties.IsHeadersPresent() && _context.Properties.Headers != null)
            {
                foreach (KeyValuePair<string, object> header in _context.Properties.Headers)
                {
                    var value = header.Value;

                    if (value is byte[] bytes)
                    {
                        var text = Encoding.UTF8.GetString(bytes);

                        if (!string.IsNullOrWhiteSpace(text))
                            yield return new KeyValuePair<string, object>(header.Key, text);
                    }
                    else if (value is string s && !string.IsNullOrWhiteSpace(s))
                        yield return new KeyValuePair<string, object>(header.Key, s);
                    else if (value != default)
                        yield return header;
                }
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_context.Properties.IsHeadersPresent() && _context.Properties.Headers != null && _context.Properties.Headers.TryGetValue(key, out value))
            {
                if (value is byte[] bytes)
                {
                    var text = Encoding.UTF8.GetString(bytes);

                    value = text;
                    return !string.IsNullOrWhiteSpace(text);
                }

                if (value is string s)
                    return !string.IsNullOrWhiteSpace(s);

                return value != default;
            }

            if (RabbitMqHeaders.Exchange.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.Exchange;
                return value != default;
            }

            if (RabbitMqHeaders.RoutingKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.RoutingKey;
                return value != default;
            }

            if (RabbitMqHeaders.DeliveryTag.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.DeliveryTag;
                return value != default;
            }

            if (RabbitMqHeaders.ConsumerTag.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.ConsumerTag;
                return value != default;
            }

            if (nameof(_context.Properties.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.Properties.MessageId;
                return value != default;
            }

            if (nameof(_context.Properties.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.Properties.CorrelationId;
                return value != default;
            }

            value = default;
            return false;
        }
    }
}
