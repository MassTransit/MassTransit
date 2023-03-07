namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text.Json;
    using Metadata;
    using Serialization;


    public class MessageDataMessageContext :
        MessageContext,
        Headers
    {
        readonly HangfireScheduledMessageData _messageData;
        readonly IObjectDeserializer _objectDeserializer;

        Guid? _conversationId;
        Guid? _correlationId;
        Uri? _destinationAddress;
        DateTime? _expirationTime;
        Uri? _faultAddress;
        Headers? _headers;
        HostInfo? _hostInfo;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri? _responseAddress;
        Uri? _sourceAddress;

        public MessageDataMessageContext(HangfireScheduledMessageData messageData, IObjectDeserializer objectDeserializer)
        {
            _messageData = messageData;
            _objectDeserializer = objectDeserializer;
        }

        public IReadOnlyDictionary<string, object>? TransportProperties
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_messageData.TransportProperties)
                    ? JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(_messageData.TransportProperties, SystemTextJsonMessageSerializer.Options)
                    : null;
            }
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return Headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Headers.GetAll();
        }

        public bool TryGetHeader(string key, [NotNullWhen(true)] out object? value)
        {
            switch (key)
            {
                case MessageHeaders.MessageId:
                    value = MessageId;
                    return value != null;
                case MessageHeaders.CorrelationId:
                    value = CorrelationId;
                    return value != null;
                case MessageHeaders.ConversationId:
                    value = ConversationId;
                    return value != null;
                case MessageHeaders.RequestId:
                    value = RequestId;
                    return value != null;
                case MessageHeaders.InitiatorId:
                    value = InitiatorId;
                    return value != null;
                case MessageHeaders.SourceAddress:
                    value = SourceAddress;
                    return value != null;
                case MessageHeaders.ResponseAddress:
                    value = ResponseAddress;
                    return value != null;
                case MessageHeaders.FaultAddress:
                    value = FaultAddress;
                    return value != null;
            }

            return Headers.TryGetHeader(key, out value);
        }

        public T? Get<T>(string key, T? defaultValue = default)
            where T : class
        {
            return TryGetHeader(key, out var value) ? _objectDeserializer.DeserializeObject(value, defaultValue) : default;
        }

        public T? Get<T>(string key, T? defaultValue = default)
            where T : struct
        {
            return TryGetHeader(key, out var value) ? _objectDeserializer.DeserializeObject(value, defaultValue) : default;
        }

        public Guid? MessageId => _messageId ??= ConvertIdToGuid(_messageData.MessageId);
        public Guid? RequestId => _requestId ??= ConvertIdToGuid(_messageData.RequestId);
        public Guid? CorrelationId => _correlationId ??= ConvertIdToGuid(_messageData.CorrelationId);
        public Guid? ConversationId => _conversationId ??= ConvertIdToGuid(_messageData.ConversationId);
        public Guid? InitiatorId => _initiatorId ??= ConvertIdToGuid(_messageData.InitiatorId);
        public Uri? SourceAddress => _sourceAddress ??= ConvertToUri(_messageData.SourceAddress);
        public Uri? DestinationAddress => _destinationAddress ??= ConvertToUri(_messageData.DestinationAddress);
        public Uri? ResponseAddress => _responseAddress ??= ConvertToUri(_messageData.ResponseAddress);
        public Uri? FaultAddress => _faultAddress ??= ConvertToUri(_messageData.FaultAddress);
        public DateTime? ExpirationTime => _expirationTime ??= ConvertDateTime(_messageData.ExpirationTime);
        public DateTime? SentTime => default;
        public Headers Headers => _headers ??= GetHeaders();
        public HostInfo Host => _hostInfo ??= HostMetadataCache.Host;

        Headers GetHeaders()
        {
            var headers = new DictionarySendHeaders();

            if (_messageData.HeadersAsJson != null)
            {
                var headerElements = JsonSerializer.Deserialize<IEnumerable<KeyValuePair<string, object>>>(_messageData.HeadersAsJson,
                    SystemTextJsonMessageSerializer.Options);
                if (headerElements != null)
                {
                    foreach (KeyValuePair<string, object> element in headerElements)
                        headers.Set(element.Key, element.Value);
                }
            }

            headers.Set(HangfireMessageHeaders.Sent, DateTime.UtcNow);

            Guid? tokenId = ConvertIdToGuid(_messageData.TokenId);
            if (tokenId.HasValue)
                headers.Set(MessageHeaders.SchedulingTokenId, tokenId);

            return headers;
        }

        static DateTime? ConvertDateTime(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return default;

            return DateTime.TryParse(text, null, DateTimeStyles.RoundtripKind, out var expirationTime)
                || DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out expirationTime)
                    ? expirationTime
                    : default(DateTime?);
        }

        static Guid? ConvertIdToGuid(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        static Uri? ConvertToUri(string? uri)
        {
            return string.IsNullOrWhiteSpace(uri) ? null : new Uri(uri);
        }
    }
}
