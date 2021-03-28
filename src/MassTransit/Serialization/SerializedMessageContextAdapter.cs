namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using GreenPipes;
    using Newtonsoft.Json;


    public class SerializedMessageContextAdapter :
        IPipe<SendContext>
    {
        readonly SerializedMessage _message;
        readonly IPipe<SendContext> _pipe;
        readonly Uri _sourceAddress;

        public SerializedMessageContextAdapter(IPipe<SendContext> pipe, SerializedMessage message, Uri sourceAddress)
        {
            _pipe = pipe;
            _message = message;
            _sourceAddress = sourceAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext context)
        {
            context.DestinationAddress = _message.Destination;
            context.SourceAddress = _sourceAddress;

            context.ResponseAddress = ToUri(_message.ResponseAddress);
            context.FaultAddress = ToUri(_message.FaultAddress);

            SetHeaders(context);

            context.MessageId = ConvertIdToGuid(_message.MessageId);
            context.RequestId = ConvertIdToGuid(_message.RequestId);
            context.CorrelationId = ConvertIdToGuid(_message.CorrelationId);
            context.ConversationId = ConvertIdToGuid(_message.ConversationId);
            context.InitiatorId = ConvertIdToGuid(_message.InitiatorId);

            if (!string.IsNullOrWhiteSpace(_message.ExpirationTime))
            {
                if (DateTime.TryParse(_message.ExpirationTime, null, DateTimeStyles.RoundtripKind, out var expirationTime)
                    || DateTime.TryParse(_message.ExpirationTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out expirationTime))
                {
                    var timeToLive = expirationTime - DateTime.UtcNow;
                    context.TimeToLive = timeToLive > TimeSpan.Zero
                        ? timeToLive
                        : TimeSpan.FromSeconds(1);
                }
            }

            var bodySerializer = new StringMessageSerializer(new ContentType(_message.ContentType), _message.Body);

            if (!string.IsNullOrWhiteSpace(_message.PayloadMessageHeadersAsJson))
            {
                IDictionary<string, object> headers = ToHeaderDictionary(_message.PayloadMessageHeadersAsJson);

                if (string.Compare(_message.ContentType, JsonMessageSerializer.JsonContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                    bodySerializer.UpdateJsonHeaders(headers);
                else if (string.Compare(_message.ContentType, XmlMessageSerializer.XmlContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                    bodySerializer.UpdateXmlHeaders(headers);
            }

            context.Serializer = bodySerializer;

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);
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
            if (string.IsNullOrEmpty(_message.HeadersAsJson))
                return;

            IDictionary<string, object> headers = ToHeaderDictionary(_message.HeadersAsJson);
            foreach (KeyValuePair<string, object> header in headers)
                context.Headers.Set(header.Key, header.Value);
        }

        static IDictionary<string, object> ToHeaderDictionary(string json)
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(json, JsonMessageSerializer.DeserializerSettings);
        }

        static Uri ToUri(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            try
            {
                return new Uri(s);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
