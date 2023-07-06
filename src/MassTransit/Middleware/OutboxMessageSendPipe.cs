#nullable enable
namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
    using Transports;


    public class OutboxMessageSendPipe :
        IPipe<SendContext>
    {
        readonly Uri? _destinationAddress;

        readonly OutboxMessageContext _message;

        public OutboxMessageSendPipe(OutboxMessageContext message, Uri? destinationAddress)
        {
            _message = message;
            _destinationAddress = destinationAddress;
        }

        public Task Send(SendContext context)
        {
            var contentType = new ContentType(_message.ContentType);

            var deserializer = context.Serialization.GetMessageDeserializer(contentType);

            var body = deserializer.GetMessageBody(_message.Body);

            var headers = new JsonTransportHeaders(new OutboxMessageHeaderProvider(_message));

            var serializerContext = deserializer.Deserialize(body, headers, _destinationAddress);

            context.MessageId = _message.MessageId;
            context.RequestId = _message.RequestId;
            context.ConversationId = _message.ConversationId;
            context.CorrelationId = _message.CorrelationId;
            context.InitiatorId = _message.InitiatorId;
            context.SourceAddress = _message.SourceAddress;
            context.ResponseAddress = _message.ResponseAddress;
            context.FaultAddress = _message.FaultAddress;
            context.SupportedMessageTypes = string.IsNullOrWhiteSpace(_message.MessageType)
                ? serializerContext.SupportedMessageTypes
                : _message.MessageType.Split(';').ToArray();

            if (_message.ExpirationTime.HasValue)
                context.TimeToLive = _message.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

            foreach (KeyValuePair<string, object> header in serializerContext.Headers.GetAll())
                context.Headers.Set(header.Key, header.Value);

            if (_message.Properties.Count > 0 && context is TransportSendContext transportSendContext)
                transportSendContext.ReadPropertiesFrom(_message.Properties);

            context.Serializer = serializerContext.GetMessageSerializer();

            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
        }


        class OutboxMessageHeaderProvider :
            IHeaderProvider
        {
            readonly OutboxMessageContext _message;

            public OutboxMessageHeaderProvider(OutboxMessageContext message)
            {
                _message = message;
            }

            public IEnumerable<KeyValuePair<string, object>> GetAll()
            {
                yield return new KeyValuePair<string, object>(MessageHeaders.MessageId, _message.MessageId);

                if (!string.IsNullOrWhiteSpace(_message.ContentType))
                    yield return new KeyValuePair<string, object>(MessageHeaders.ContentType, _message.ContentType!);

                foreach (KeyValuePair<string, object> header in _message.Headers.GetAll())
                {
                    switch (header.Key)
                    {
                        case MessageHeaders.MessageId:
                        case MessageHeaders.ContentType:
                            continue;

                        default:
                            yield return header;
                            break;
                    }
                }
            }

            public bool TryGetHeader(string key, [NotNullWhen(true)] out object? value)
            {
                if (nameof(_message.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    value = _message.MessageId;
                    return true;
                }

                if (MessageHeaders.ContentType.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    value = _message.ContentType;
                    return true;
                }

                if (_message.Headers.TryGetHeader(key, out var headerValue))
                {
                    value = headerValue;
                    return true;
                }

                value = null;
                return false;
            }
        }
    }
}
