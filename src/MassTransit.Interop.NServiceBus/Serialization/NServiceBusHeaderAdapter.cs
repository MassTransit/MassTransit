namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;


    public class NServiceBusHeaderAdapter :
        MessageContext
    {
        readonly Headers _headers;
        Guid? _conversationId;
        Guid? _correlationId;
        Headers _headerFilter;
        HostInfo _host;
        Guid? _messageId;
        string[] _messageTypes;
        Uri _responseAddress;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public NServiceBusHeaderAdapter(Headers headers)
        {
            _headers = headers;
        }

        public string[] SupportedMessageTypes => _messageTypes ??= GetMessageTypes().ToArray();

        public Guid? MessageId => _messageId ??= _headers.Get<Guid>(NServiceBusMessageHeaders.MessageId);

        public Guid? RequestId { get; } = default;

        public Guid? CorrelationId => _correlationId ??= _headers.Get<Guid>(NServiceBusMessageHeaders.CorrelationId);
        public Guid? ConversationId => _conversationId ??= _headers.Get<Guid>(NServiceBusMessageHeaders.ConversationId);

        public Guid? InitiatorId { get; } = default;

        public DateTime? ExpirationTime { get; }

        public Uri SourceAddress => _sourceAddress ??= GetEndpointAddress(NServiceBusMessageHeaders.OriginatingEndpoint);

        public Uri DestinationAddress { get; } = default;

        public Uri ResponseAddress => _responseAddress ??= GetEndpointAddress(NServiceBusMessageHeaders.ReplyToAddress);

        public Uri FaultAddress { get; } = default;

        public DateTime? SentTime => _sentTime ??= GetSentTime();

        public Headers Headers => _headerFilter ??= new TransportHeaderFilter(_headers);

        public HostInfo Host => _host ??= GetHostInfo();

        Uri GetEndpointAddress(string key)
        {
            var endpoint = _headers.Get<string>(key);
            return string.IsNullOrWhiteSpace(endpoint)
                ? default
                : new Uri($"queue:{endpoint}");
        }

        DateTime? GetSentTime()
        {
            var timeSent = _headers.Get<string>(NServiceBusMessageHeaders.TimeSent);
            if (string.IsNullOrWhiteSpace(timeSent))
                return default;

            if (timeSent.Length >= 28)
                timeSent = timeSent.Substring(0, 19) + '.' + timeSent.Substring(20, 6);

            return DateTime.TryParse(timeSent, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var value)
                ? value
                : default(DateTime?);
        }

        IEnumerable<string> GetMessageTypes()
        {
            var enclosedMessageTypes = _headers.Get<string>(NServiceBusMessageHeaders.EnclosedMessageTypes);
            if (string.IsNullOrWhiteSpace(enclosedMessageTypes))
                yield break;

            string[] typeNames = enclosedMessageTypes.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var typeName in typeNames)
            {
                var messageType = Type.GetType(typeName);
                if (messageType != null)
                    yield return MessageUrn.ForType(messageType).ToString();
            }
        }

        HostInfo GetHostInfo()
        {
            var machineName = _headers.Get<string>(NServiceBusMessageHeaders.OriginatingMachine);
            var version = _headers.Get<string>(NServiceBusMessageHeaders.Version);
            if (!string.IsNullOrWhiteSpace(version))
                version = $"NServiceBus {version}";

            return new NServiceBusHostInfo(machineName, version);
        }


        class NServiceBusHostInfo :
            HostInfo
        {
            public NServiceBusHostInfo(string machineName, string version)
            {
                MachineName = machineName;
                MassTransitVersion = version;
            }

            public string MachineName { get; }
            public string ProcessName => default;
            public int ProcessId => default;
            public string Assembly => default;
            public string AssemblyVersion => default;
            public string FrameworkVersion => default;
            public string MassTransitVersion { get; }
            public string OperatingSystemVersion => default;
        }


        class TransportHeaderFilter :
            Headers
        {
            readonly Headers _headers;

            public TransportHeaderFilter(Headers headers)
            {
                _headers = headers;
            }

            public IEnumerator<HeaderValue> GetEnumerator()
            {
                return GetAll().Select(x => new HeaderValue(x.Key, x.Value)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_headers).GetEnumerator();
            }

            public IEnumerable<KeyValuePair<string, object>> GetAll()
            {
                foreach (KeyValuePair<string, object> header in _headers.GetAll())
                {
                    if (header.Key.StartsWith("NServiceBus."))
                        continue;

                    if (header.Key.StartsWith("RabbitMQ-"))
                        continue;

                    switch (header.Key)
                    {
                        case NServiceBusMessageHeaders.DiagnosticsOriginatingHostId:
                        case "publishId":
                            break;

                        default:
                            yield return header;
                            break;
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
