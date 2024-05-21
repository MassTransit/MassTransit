namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
    using Transports;


    public class AmazonSqsHeaderProvider :
        IHeaderProvider
    {
        readonly SqsMessageBody _body;
        readonly Message _message;

        public AmazonSqsHeaderProvider(Message message, SqsMessageBody body)
        {
            _message = message;
            _body = body;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_message.MessageAttributes.TryGetValue(key, out var val))
            {
                value = val.StringValue;
                return value != null;
            }

            if (nameof(Message.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.MessageId;
                return true;
            }

            if ("TopicArn".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _body.TopicArn;
                return value != null;
            }

            if (MessageHeaders.TransportSentTime.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                if (_message.Attributes.TryGetValue(MessageSystemAttributeName.SentTimestamp, out var sentTimestamp))
                {
                    if (long.TryParse(sentTimestamp, out var milliseconds))
                    {
                        value = DateTimeConstants.Epoch + TimeSpan.FromMilliseconds(milliseconds);
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (!TryGetHeader(MessageHeaders.MessageId, out _))
                yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _message.MessageId);

            foreach (KeyValuePair<string, object> header in _message.MessageAttributes
                         .Where(x => x.Value.StringValue != null)
                         .Select(x => new KeyValuePair<string, object>(x.Key, x.Value.StringValue)))
                yield return header;
        }
    }
}
