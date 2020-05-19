namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Amazon.SQS.Model;
    using Context;


    public class AmazonSqsHeaderProvider :
        IHeaderProvider
    {
        readonly Message _message;
        readonly Headers _headers;

        public AmazonSqsHeaderProvider(Message message)
        {
            _message = message;

            _headers = new AmazonSqsHeaders(message.MessageAttributes);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _message.MessageId);

            foreach (var header in _headers.GetAll())
                yield return header;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (nameof(Message.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.MessageId;
                return true;
            }

            return _headers.TryGetHeader(key, out value);
        }
    }
}
