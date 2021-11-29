namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class RawMessageContext :
        MessageContext
    {
        static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1);

        readonly RawSerializerOptions _options;
        readonly Headers _transportHeaders;

        Guid? _conversationId;
        Guid? _correlationId;
        Uri _faultAddress;
        Headers _headers;
        HostInfo _host;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public RawMessageContext(Headers headers, Uri destinationAddress, RawSerializerOptions options)
        {
            _transportHeaders = headers;
            DestinationAddress = destinationAddress;
            _options = options;
        }

        public Guid? MessageId => _messageId ??= _transportHeaders.GetMessageId();
        public Guid? RequestId => _requestId ??= _transportHeaders.GetRequestId();
        public Guid? CorrelationId => _correlationId ??= _transportHeaders.GetCorrelationId();
        public Guid? ConversationId => _conversationId ??= _transportHeaders.GetConversationId();
        public Guid? InitiatorId => _initiatorId ??= _transportHeaders.GetInitiatorId();
        public DateTime? ExpirationTime { get; } = default;
        public Uri SourceAddress => _sourceAddress ??= _transportHeaders.GetSourceAddress();
        public Uri DestinationAddress { get; }
        public Uri ResponseAddress => _responseAddress ??= _transportHeaders.GetResponseAddress();
        public Uri FaultAddress => _faultAddress ??= _transportHeaders.GetFaultAddress();

        public DateTime? SentTime => _sentTime ??= GetSentTime();

        public Headers Headers => _headers ??= new TransportHeaderFilter(_transportHeaders, _options);

        public HostInfo Host => _host ??= GetHostInfo();

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

        HostInfo GetHostInfo()
        {
            return _transportHeaders.Get<HostInfo>(MessageHeaders.Host.Info);
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
                            case MessageHeaders.InitiatorId:
                            case MessageHeaders.SourceAddress:
                            case MessageHeaders.ResponseAddress:
                            case MessageHeaders.FaultAddress:
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
