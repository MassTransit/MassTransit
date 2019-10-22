namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Context;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Util;


    public class JsonConsumeContext :
        DeserializerConsumeContext
    {
        readonly PendingTaskCollection _consumeTasks;
        readonly JsonSerializer _deserializer;
        readonly MessageEnvelope _envelope;
        readonly JToken _messageToken;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;
        readonly string[] _supportedTypes;

        Guid? _conversationId;
        Guid? _correlationId;
        Uri _destinationAddress;
        Uri _faultAddress;
        Headers _headers;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public JsonConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, MessageEnvelope envelope)
            : base(receiveContext)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _envelope = envelope;
            _deserializer = deserializer;
            _messageToken = GetMessageToken(envelope.Message);
            _supportedTypes = envelope.MessageType.ToArray();
            _messageTypes = new Dictionary<Type, ConsumeContext>();
            _consumeTasks = new PendingTaskCollection(4);
        }

        public override Task ConsumeCompleted => _consumeTasks.Completed(CancellationToken);

        public override Guid? MessageId => _messageId ?? (_messageId = ConvertIdToGuid(_envelope.MessageId));
        public override Guid? RequestId => _requestId ?? (_requestId = ConvertIdToGuid(_envelope.RequestId));
        public override Guid? CorrelationId => _correlationId ?? (_correlationId = ConvertIdToGuid(_envelope.CorrelationId));
        public override Guid? ConversationId => _conversationId ?? (_conversationId = ConvertIdToGuid(_envelope.ConversationId));
        public override Guid? InitiatorId => _initiatorId ?? (_initiatorId = ConvertIdToGuid(_envelope.InitiatorId));
        public override DateTime? ExpirationTime => _envelope.ExpirationTime;
        public override Uri SourceAddress => _sourceAddress ?? (_sourceAddress = ConvertToUri(_envelope.SourceAddress));
        public override Uri DestinationAddress => _destinationAddress ?? (_destinationAddress = ConvertToUri(_envelope.DestinationAddress));
        public override Uri ResponseAddress => _responseAddress ?? (_responseAddress = ConvertToUri(_envelope.ResponseAddress));
        public override Uri FaultAddress => _faultAddress ?? (_faultAddress = ConvertToUri(_envelope.FaultAddress));
        public override DateTime? SentTime => _envelope.SentTime;
        public override Headers Headers => _headers ?? (_headers = new JsonMessageHeaders(_envelope.Headers));
        public override HostInfo Host => _envelope.Host;
        public override IEnumerable<string> SupportedMessageTypes => _supportedTypes;

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            string typeUrn = MessageUrn.ForTypeString(messageType);

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

                if (typeof(T) == typeof(JToken))
                {
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, _messageToken as T);
                    return true;
                }

                string typeUrn = MessageUrn.ForTypeString<T>();

                if (_supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    object obj;
                    Type deserializeType = typeof(T);
                    if (deserializeType.GetTypeInfo().IsInterface && TypeMetadataCache<T>.IsValidMessageType)
                        deserializeType = TypeMetadataCache<T>.ImplementationType;

                    using (JsonReader jsonReader = _messageToken.CreateReader())
                    {
                        obj = _deserializer.Deserialize(jsonReader, deserializeType);
                    }

                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)obj);
                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }

        public override void AddConsumeTask(Task task)
        {
            _consumeTasks.Add(task);
        }

        static JToken GetMessageToken(object message)
        {
            var messageToken = message as JToken;
            if (messageToken == null || messageToken.Type == JTokenType.Null)
                return new JObject();

            return messageToken;
        }

        /// <summary>
        ///     Converts a string identifier to a Guid, if it's actually a Guid. Can throw a FormatException
        ///     if things are not right
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        /// <summary>
        ///     Convert the string to a Uri, or return null if it's empty
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static Uri ConvertToUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;

            return new Uri(uri);
        }
    }
}
