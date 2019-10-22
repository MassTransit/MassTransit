namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading.Tasks;
    using Context;
    using Util;


    /// <summary>
    /// A static consume context from the Binary serializer
    /// </summary>
    public class StaticConsumeContext :
        DeserializerConsumeContext
    {
        readonly Header[] _binaryHeaders;
        readonly object _message;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;
        readonly string[] _supportedTypes;
        readonly PendingTaskCollection _consumeTasks;
        Guid? _conversationId;
        Guid? _correlationId;
        Uri _destinationAddress;
        Uri _faultAddress;
        Headers _headers;
        HostInfo _host;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;
        DateTime? _sentTime;

        public StaticConsumeContext(ReceiveContext receiveContext, object message, Header[] headers)
            : base(receiveContext)
        {
            _message = message;
            _binaryHeaders = headers;
            _supportedTypes = GetSupportedMessageTypes().ToArray();
            _messageTypes = new Dictionary<Type, ConsumeContext>();
            _consumeTasks = new PendingTaskCollection(4);
        }

        public override Task ConsumeCompleted => _consumeTasks.Completed(CancellationToken);

        public override Guid? MessageId => _messageId ?? (_messageId = GetHeaderGuid(BinaryMessageSerializer.MessageIdKey));
        public override Guid? RequestId => _requestId ?? (_requestId = GetHeaderGuid(BinaryMessageSerializer.RequestIdKey));
        public override Guid? CorrelationId => _correlationId ?? (_correlationId = GetHeaderGuid(BinaryMessageSerializer.CorrelationIdKey));
        public override Guid? ConversationId => _conversationId ?? (_conversationId = GetHeaderGuid(BinaryMessageSerializer.ConversationIdKey));
        public override Guid? InitiatorId => _initiatorId ?? (_initiatorId = GetHeaderGuid(BinaryMessageSerializer.InitiatorIdKey));
        public override DateTime? ExpirationTime => GetHeaderDateTime(BinaryMessageSerializer.ExpirationTimeKey);
        public override Uri SourceAddress => _sourceAddress ?? (_sourceAddress = GetHeaderUri(BinaryMessageSerializer.SourceAddressKey));
        public override Uri DestinationAddress => _destinationAddress ?? (_destinationAddress = GetHeaderUri(BinaryMessageSerializer.DestinationAddressKey));
        public override Uri ResponseAddress => _responseAddress ?? (_responseAddress = GetHeaderUri(BinaryMessageSerializer.ResponseAddressKey));
        public override Uri FaultAddress => _faultAddress ?? (_faultAddress = GetHeaderUri(BinaryMessageSerializer.FaultAddressKey));
        public override DateTime? SentTime => _sentTime ?? (_sentTime = GetHeaderDateTime(BinaryMessageSerializer.SentTimeKey));
        public override Headers Headers => _headers ?? (_headers = new StaticHeaders(_binaryHeaders));
        public override HostInfo Host => _host ?? (_host = GetHeaderObject<HostInfo>(BinaryMessageSerializer.HostInfoKey));
        public override IEnumerable<string> SupportedMessageTypes => _supportedTypes;

        IEnumerable<string> GetSupportedMessageTypes()
        {
            yield return GetHeaderString(BinaryMessageSerializer.MessageTypeKey);
            var header = GetHeaderString(BinaryMessageSerializer.PolymorphicMessageTypesKey);
            if (header != null)
            {
                string[] additionalMessageUrns = header.Split(';');
                foreach (var additionalMessageUrn in additionalMessageUrns)
                {
                    yield return additionalMessageUrn;
                }
            }
        }

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            var typeUrn = MessageUrn.ForTypeString(messageType);

            return _supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> message)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(typeof(T), out var existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                var typeUrn = MessageUrn.ForTypeString<T>();

                if (_supportedTypes.Any(typeUrn.Equals))
                {
                    if (_message is T variable)
                    {
                        _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, variable);
                        return true;
                    }

                    message = null;
                    return false;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }

        public override void AddConsumeTask(Task task)
        {
            _consumeTasks.Add(task);
        }

        string GetHeaderString(string headerName)
        {
            var header = GetHeader(headerName);
            if (header == null)
                return null;

            var s = header as string;
            if (s != null)
                return s;

            var uri = header as Uri;
            if (uri != null)
                return uri.ToString();

            return header.ToString();
        }

        Uri GetHeaderUri(string headerName)
        {
            try
            {
                var header = GetHeader(headerName);
                if (header == null)
                    return null;

                var uri = header as Uri;
                if (uri != null)
                    return uri;

                var s = header as string;
                if (s != null)
                    return new Uri(s);
            }
            catch (UriFormatException)
            {
            }

            return null;
        }

        T GetHeaderObject<T>(string headerName)
            where T : class
        {
            var header = GetHeader(headerName);

            var obj = header as T;

            return obj;
        }

        Guid? GetHeaderGuid(string headerName)
        {
            try
            {
                var header = GetHeader(headerName);
                if (header == null)
                    return default;

                if (header is Guid guid)
                    return guid;

                var s = header as string;
                if (s != null)
                    return new Guid(s);
            }
            catch (FormatException)
            {
            }

            return default;
        }

        DateTime? GetHeaderDateTime(string headerName)
        {
            try
            {
                var header = GetHeader(headerName);
                if (header == null)
                    return default;

                if (header is DateTime time)
                    return time;

                var s = header as string;
                if (s != null)
                    return DateTime.Parse(s);
            }
            catch (FormatException)
            {
            }

            return default;
        }

        object GetHeader(string headerName)
        {
            return _binaryHeaders.Where(x => x.Name == headerName).Select(x => x.Value).FirstOrDefault();
        }
    }
}
