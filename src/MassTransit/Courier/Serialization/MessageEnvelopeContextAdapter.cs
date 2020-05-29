namespace MassTransit.Courier.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Serialization;


    public class MessageEnvelopeContextAdapter :
        IPipe<SendContext>
    {
        readonly string _contentType;
        readonly MessageEnvelope _envelope;
        readonly IPipe<SendContext> _pipe;
        readonly object _subscriptionMessage;

        public MessageEnvelopeContextAdapter(IPipe<SendContext> pipe, MessageEnvelope envelope, string contentType, object subscriptionMessage)
        {
            _pipe = pipe;
            _envelope = envelope;
            _contentType = contentType;
            _subscriptionMessage = subscriptionMessage;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext context)
        {
            context.DestinationAddress = ToUri(_envelope.DestinationAddress);

            context.ResponseAddress = ToUri(_envelope.ResponseAddress);
            context.FaultAddress = ToUri(_envelope.FaultAddress);

            SetHeaders(context);

            context.RequestId = ConvertIdToGuid(_envelope.RequestId);
            context.CorrelationId = ConvertIdToGuid(_envelope.CorrelationId);

            if (_envelope.ExpirationTime.HasValue)
                context.TimeToLive = _envelope.ExpirationTime.Value - DateTime.UtcNow;

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);

            var bodySerializer = new EnvelopeMessageSerializer(new ContentType(_contentType), _envelope, _subscriptionMessage);

            context.Serializer = bodySerializer;
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        void SetHeaders(SendContext context)
        {
            foreach (KeyValuePair<string, object> header in _envelope.Headers)
                context.Headers.Set(header.Key, header.Value);
        }

        static Uri ToUri(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            return new Uri(s);
        }
    }
}
