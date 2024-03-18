namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Transports;


    public class KafkaHeaderProvider :
        IHeaderProvider
    {
        readonly Message<byte[], byte[]> _message;
        readonly IHeaderProvider _provider;

        public KafkaHeaderProvider(Message<byte[], byte[]> message, IHeaderProvider provider)
        {
            _message = message;
            _provider = provider;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _provider.GetAll();
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (MessageHeaders.TransportSentTime.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.Timestamp.UtcDateTime;
                return true;
            }

            return _provider.TryGetHeader(key, out value);
        }
    }
}
