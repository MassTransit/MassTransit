namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Metadata;
    using Quartz;
    using Serialization;


    public class JobDataMessageContext :
        MessageContext,
        Headers
    {
        readonly IJobExecutionContext _executionContext;
        readonly JobDataMap _jobDataMap;
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
        DateTime? _sentTime;
        Uri? _sourceAddress;

        public JobDataMessageContext(IJobExecutionContext executionContext, IObjectDeserializer objectDeserializer)
        {
            _executionContext = executionContext;
            _jobDataMap = executionContext.MergedJobDataMap;
            _objectDeserializer = objectDeserializer;

            Guid? messageId = _jobDataMap.TryGetString(nameof(MessageId), out var text) ? ConvertIdToGuid(text) : default;

            if (messageId.HasValue)
                _messageId = messageId;
            else
            {
                var newId = NewId.Next();

                _messageId = newId.ToGuid();
                _sentTime = newId.Timestamp;
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

            return _jobDataMap.TryGetValue(key, out value);
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

        public Guid? MessageId => _messageId ??= _jobDataMap.TryGetValue(nameof(MessageId), out string value) ? ConvertIdToGuid(value) : NewId.NextGuid();
        public Guid? RequestId => _requestId ??= _jobDataMap.TryGetValue(nameof(RequestId), out string value) ? ConvertIdToGuid(value) : default;
        public Guid? CorrelationId => _correlationId ??= _jobDataMap.TryGetValue(nameof(CorrelationId), out string value) ? ConvertIdToGuid(value) : default;
        public Guid? ConversationId => _conversationId ??= _jobDataMap.TryGetValue(nameof(ConversationId), out string value) ? ConvertIdToGuid(value) : default;
        public Guid? InitiatorId => _initiatorId ??= _jobDataMap.TryGetValue(nameof(InitiatorId), out string value) ? ConvertIdToGuid(value) : default;

        public DateTime? ExpirationTime =>
            _expirationTime ??= _jobDataMap.TryGetValue(nameof(ExpirationTime), out string value) ? ConvertDateTime(value) : default;

        public Uri? SourceAddress => _sourceAddress ??= _jobDataMap.TryGetValue(nameof(SourceAddress), out string value) ? ConvertToUri(value) : default;

        public Uri? DestinationAddress =>
            _destinationAddress ??= _jobDataMap.TryGetValue(nameof(DestinationAddress), out string value) ? ConvertToUri(value) : default;

        public Uri? ResponseAddress => _responseAddress ??= _jobDataMap.TryGetValue(nameof(ResponseAddress), out string value) ? ConvertToUri(value) : default;
        public Uri? FaultAddress => _faultAddress ??= _jobDataMap.TryGetValue(nameof(FaultAddress), out string value) ? ConvertToUri(value) : default;
        public DateTime? SentTime => _sentTime ??= _jobDataMap.TryGetValue(nameof(SentTime), out DateTime? value) ? value : default;
        public Headers Headers => _headers ??= GetHeaders();
        public HostInfo Host => _hostInfo ??= _jobDataMap.TryGetValue(nameof(Host), out HostInfo? value) ? value! : HostMetadataCache.Empty;

        public IReadOnlyDictionary<string, object>? TransportProperties =>
            _jobDataMap.TryGetValue<IReadOnlyDictionary<string, object>>("TransportProperties", out var properties) ? properties : default;

        Headers GetHeaders()
        {
            var headers = new DictionarySendHeaders();

            if (_jobDataMap.TryGetValue("HeadersAsJson", out IEnumerable<KeyValuePair<string, object>> headerElements))
            {
                foreach (KeyValuePair<string, object> element in headerElements)
                    headers.Set(element.Key, element.Value);
            }

            headers.Set(MessageHeaders.Quartz.Sent, _executionContext.FireTimeUtc);

            if (_executionContext.ScheduledFireTimeUtc.HasValue)
                headers.Set(MessageHeaders.Quartz.Scheduled, _executionContext.ScheduledFireTimeUtc);

            if (_executionContext.NextFireTimeUtc.HasValue)
                headers.Set(MessageHeaders.Quartz.NextScheduled, _executionContext.NextFireTimeUtc);

            if (_executionContext.PreviousFireTimeUtc.HasValue)
                headers.Set(MessageHeaders.Quartz.PreviousSent, _executionContext.PreviousFireTimeUtc);

            if (_jobDataMap.TryGetValue("TokenId", out var tokenId))
                headers.Set(MessageHeaders.SchedulingTokenId, tokenId);

            if (!string.IsNullOrWhiteSpace(_executionContext.Trigger.Key.Name))
                headers.Set(MessageHeaders.Quartz.ScheduleId, _executionContext.Trigger.Key.Name.Replace(QuartzConstants.RecurringTriggerPrefix, ""));

            if (!string.IsNullOrWhiteSpace(_executionContext.Trigger.Key.Group))
                headers.Set(MessageHeaders.Quartz.ScheduleGroup, _executionContext.Trigger.Key.Group);

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
