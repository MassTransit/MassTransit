using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MassTransit.Serialization
{
    public class RawXmlHeaderAdapter
    {
        static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1);

        readonly Headers _headers;
        readonly RawXmlSerializerOptions _options;
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

        public RawXmlHeaderAdapter(ReceiveContext receiveContext, RawXmlSerializerOptions options)
        {
            _receiveContext = receiveContext;
            _options = options;
            _headers = receiveContext.TransportHeaders;
        }

        public static T NewIfNull<T>(T current, T newObject)
        {
            if (current == null)
            {
                return newObject;
            }
            return current;
        }
        public string[] SupportedMessageTypes => NewIfNull(_messageTypes, GetMessageTypes().ToArray());

        public Guid? MessageId => NewIfNull(_messageId, _receiveContext.GetMessageId());
        public Guid? RequestId => NewIfNull(_requestId, _receiveContext.GetRequestId());
        public Guid? CorrelationId => NewIfNull(_correlationId, _receiveContext.GetCorrelationId());
        public Guid? ConversationId => NewIfNull(_conversationId, _receiveContext.GetConversationId());
        public Guid? InitiatorId => NewIfNull(_initiatorId, _receiveContext.GetInitiatorId());

        public DateTime? ExpirationTime { get; } = default;

        public Uri SourceAddress => NewIfNull(_sourceAddress, GetEndpointAddress(MessageHeaders.SourceAddress));

        public Uri DestinationAddress => _receiveContext.InputAddress;
        public Uri ResponseAddress => NewIfNull(_responseAddress, GetEndpointAddress(MessageHeaders.ResponseAddress));
        public Uri FaultAddress => NewIfNull(_faultAddress, GetEndpointAddress(MessageHeaders.FaultAddress));

        public DateTime? SentTime => NewIfNull(_sentTime, GetSentTime());

        public Headers Headers => NewIfNull(_headerFilter, new TransportHeaderFilter(_headers, new RawJsonSerializerOptions()));

        public HostInfo Host => NewIfNull(_host, GetHostInfo());

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
            readonly RawJsonSerializerOptions _options;

            public TransportHeaderFilter(Headers headers, RawJsonSerializerOptions options)
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
                if (_options.HasFlag(RawJsonSerializerOptions.CopyHeaders))
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
