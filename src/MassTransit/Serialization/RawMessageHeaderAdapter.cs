namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class RawMessageHeaderAdapter :
        MessageContext
    {
        static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1);

        readonly Headers _headers;
        readonly RawSerializerOptions _options;
        readonly ReceiveContext _receiveContext;
        Guid? _conversationId;
        Guid? _correlationId;
        Uri _faultAddress;
        Headers _headerFilter;
        HostInfo _host;
        Guid? _initiatorId;
        Guid? _messageId;
        string[] _messageTypes;
        Guid? _requestId;
        Uri _responseAddress;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public RawMessageHeaderAdapter(ReceiveContext receiveContext, RawSerializerOptions options)
        {
            _receiveContext = receiveContext;
            _options = options;
            _headers = receiveContext.TransportHeaders;
        }

        public string[] SupportedMessageTypes => _messageTypes ??= GetMessageTypes().ToArray();

        public Guid? MessageId => _messageId ??= _receiveContext.GetMessageId();
        public Guid? RequestId => _requestId ??= _receiveContext.GetRequestId();
        public Guid? CorrelationId => _correlationId ??= _receiveContext.GetCorrelationId();
        public Guid? ConversationId => _conversationId ??= _receiveContext.GetConversationId();
        public Guid? InitiatorId => _initiatorId ??= _receiveContext.GetInitiatorId();

        public DateTime? ExpirationTime { get; } = default;

        public Uri SourceAddress => _sourceAddress ??= GetEndpointAddress(MessageHeaders.SourceAddress);
        public Uri DestinationAddress => _receiveContext.InputAddress;
        public Uri ResponseAddress => _responseAddress ??= GetEndpointAddress(MessageHeaders.ResponseAddress);
        public Uri FaultAddress => _faultAddress ??= GetEndpointAddress(MessageHeaders.FaultAddress);

        public DateTime? SentTime => _sentTime ??= GetSentTime();

        public Headers Headers => _headerFilter ??= new TransportHeaderFilter(_headers, _options);

        public HostInfo Host => _host ??= GetHostInfo();

        Uri GetEndpointAddress(string key)
        {
            try
            {
                var address = _headers.Get<string>(key);
                return string.IsNullOrWhiteSpace(address)
                    ? default
                    : new Uri(address);
            }
            catch (FormatException)
            {
                return default;
            }
        }

        DateTime? GetSentTime()
        {
            try
            {
                DateTime? sentTime = MessageId?.ToNewId().Timestamp;

                return sentTime > _unixEpoch ? sentTime : default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        IEnumerable<string> GetMessageTypes()
        {
            var messageTypes = _headers.Get<string>(MessageHeaders.MessageType);

            return string.IsNullOrWhiteSpace(messageTypes) ? Enumerable.Empty<string>() : messageTypes.Split(';');
        }

        HostInfo GetHostInfo()
        {
            return _headers.Get<HostInfo>(MessageHeaders.Host.Info);
        }


        class TransportHeaderFilter :
            Headers
        {
            readonly Headers _headers;
            readonly RawSerializerOptions _options;

            public TransportHeaderFilter(Headers headers, RawSerializerOptions options)
            {
                _headers = headers;
                _options = options;
            }

            public IEnumerator<HeaderValue> GetEnumerator()
            {
                return GetAll().Select(x => new HeaderValue(x.Key, x.Value)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerable<KeyValuePair<string, object>> GetAll()
            {
                if (_options.HasFlag(RawSerializerOptions.CopyHeaders))
                {
                    foreach (KeyValuePair<string, object> header in _headers.GetAll())
                    {
                        if (header.Key.StartsWith("MT-"))
                            continue;

                        switch (header.Key)
                        {
                            case MessageHeaders.MessageId:
                            case MessageHeaders.CorrelationId:
                            case MessageHeaders.ConversationId:
                            case MessageHeaders.RequestId:
                                break;

                            default:
                                yield return header;
                                break;
                        }
                    }
                }
            }

            public bool TryGetHeader(string key, out object value)
            {
                return _headers.TryGetHeader(key, out value);
            }

            public T Get<T>(string key, T defaultValue = default)
                where T : class
            {
                return _headers.Get(key, defaultValue);
            }

            public T? Get<T>(string key, T? defaultValue = null)
                where T : struct
            {
                return _headers.Get(key, defaultValue);
            }
        }
    }
}
