#nullable enable
namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

            if (serializerContext.MessageId.HasValue)
                context.MessageId = serializerContext.MessageId;

            context.RequestId = serializerContext.RequestId;
            context.ConversationId = serializerContext.ConversationId;
            context.CorrelationId = serializerContext.CorrelationId;
            context.InitiatorId = serializerContext.InitiatorId;
            context.SourceAddress = serializerContext.SourceAddress;
            context.ResponseAddress = serializerContext.ResponseAddress;
            context.FaultAddress = serializerContext.FaultAddress;

            if (serializerContext.ExpirationTime.HasValue)
                context.TimeToLive = serializerContext.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

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
                    return value != null;
                }

                value = null;
                return false;
            }
        }
    }
}
