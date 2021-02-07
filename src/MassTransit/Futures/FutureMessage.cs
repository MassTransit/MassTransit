namespace MassTransit.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Courier;
    using Metadata;
    using Util;


    public class FutureMessage
    {
        public FutureMessage(IDictionary<string, object> message, string[] supportedMessageTypes)
        {
            Message = message;
            SupportedMessageTypes = supportedMessageTypes;
        }

        public IDictionary<string, object> Message { get; private set; }

        public string[] SupportedMessageTypes { get; private set; }

        public bool HasMessageType(Type messageType)
        {
            var typeUrn = MessageUrn.ForTypeString(messageType);

            return SupportedMessageTypes?.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        public bool HasMessageType<T>()
            where T : class
        {
            var typeUrn = MessageUrn.ForTypeString<T>();

            return SupportedMessageTypes?.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        public T ToObject<T>()
            where T : class
        {
            return Message != null ? ObjectTypeDeserializer.Deserialize<T>(Message) : null;
        }
    }


    public class FutureMessage<T> :
        FutureMessage
        where T : class
    {
        public FutureMessage(T result)
            : base(SerializerCache.GetObjectAsDictionary(result), TypeMetadataCache<T>.MessageTypeNames)
        {
        }
    }
}
