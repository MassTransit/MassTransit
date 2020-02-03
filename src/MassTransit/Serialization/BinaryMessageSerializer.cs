namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using Metadata;


    /// <summary>
    /// The binary message serializer used the .NET BinaryFormatter to serialize
    /// message content.
    /// </summary>
    public class BinaryMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+binary";
        public const string CorrelationIdKey = "CorrelationId";
        public const string ConversationIdKey = "ConversationId";
        public const string InitiatorIdKey = "InitiatorId";
        public const string DestinationAddressKey = "DestinationAddress";
        public const string ExpirationTimeKey = "Expiration";
        public const string FaultAddressKey = "FaultAddress";
        public const string MessageIdKey = "MessageId";
        public const string MessageTypeKey = "MessageType";
        public const string PolymorphicMessageTypesKey = "PolymorphicMessageTypes";
        public const string RequestIdKey = "RequestId";
        public const string ResponseAddressKey = "ResponseAddress";
        public const string SourceAddressKey = "SourceAddress";
        public const string SentTimeKey = "SentTime";
        public const string HostInfoKey = "HostInfo";
        public static readonly ContentType BinaryContentType = new ContentType(ContentTypeHeaderValue);

        static readonly BinaryFormatter _formatter = new BinaryFormatter();

        void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
        {
            object message = context.Message;
            if (message == null)
                throw new ArgumentNullException(nameof(context), "The message must not be null");

            Type t = message.GetType().GetTypeInfo();
            if (!t.IsSerializable)
            {
                throw new ConventionException(
                    $"Whoa, slow down buddy. The message '{TypeMetadataCache<T>.ShortName}' must be marked with the 'Serializable' attribute!");
            }

            try
            {
                _formatter.Serialize(stream, context.Message, GetHeaders<T>(context));
            }
            catch
            {
                throw new ConventionException(
                    $"The message '{TypeMetadataCache<T>.ShortName}' failed to be serialized. Check if all inner objects are serializable.");
            }

            context.ContentType = BinaryContentType;
        }

        ContentType IMessageSerializer.ContentType => BinaryContentType;

        static Header[] GetHeaders<T>(SendContext context)
        {
            var headers = new List<Header>();

            headers.Add(MessageTypeKey, MessageUrn.ForType<T>());
            headers.Add(new Header(PolymorphicMessageTypesKey, string.Join(";", TypeMetadataCache<T>.MessageTypeNames)));

            if (context.CorrelationId.HasValue)
                headers.Add(CorrelationIdKey, context.CorrelationId.Value.ToString());

            if (context.RequestId.HasValue)
                headers.Add(RequestIdKey, context.RequestId.Value.ToString());

            if (context.ConversationId.HasValue)
                headers.Add(ConversationIdKey, context.ConversationId.Value.ToString());

            if (context.InitiatorId.HasValue)
                headers.Add(InitiatorIdKey, context.InitiatorId.Value.ToString());

            headers.Add(SourceAddressKey, context.SourceAddress);
            headers.Add(DestinationAddressKey, context.DestinationAddress);

            if (context.ResponseAddress != null)
                headers.Add(ResponseAddressKey, context.ResponseAddress);

            if (context.FaultAddress != null)
                headers.Add(FaultAddressKey, context.FaultAddress);

            if (context.TimeToLive.HasValue)
                headers.Add(ExpirationTimeKey, DateTime.UtcNow + context.TimeToLive.Value);

            headers.Add(SentTimeKey, context.SentTime ?? DateTime.UtcNow);

            headers.Add(new Header(HostInfoKey, HostMetadataCache.Host));

            return headers.ToArray();
        }
    }
}
