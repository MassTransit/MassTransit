#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Serialization;


    public interface SerializerContext :
        MessageContext,
        IObjectDeserializer
    {
        string[] SupportedMessageTypes { get; }

        bool IsSupportedMessageType<T>()
            where T : class;

        bool TryGetMessage<T>(out T? message)
            where T : class;

        bool TryGetMessage(Type messageType, out object? message);

        /// <summary>
        /// Returns a message serializer using the deserialized message ContentType, that can be used to
        /// serialize the message on another <see cref="SendContext" />.
        /// </summary>
        /// <returns></returns>
        IMessageSerializer GetMessageSerializer();

        /// <summary>
        /// Returns a message serializer using the deserialized message ContentType, that can be used to
        /// serialize the message on another <see cref="SendContext" />.
        /// </summary>
        /// <param name="envelope">The message envelope to modify</param>
        /// <param name="message">A message to overlay on top of the existing message, merging the properties together</param>
        /// <returns></returns>
        IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
            where T : class;

        /// <summary>
        /// Returns a message serializer using the deserialized message ContentType, that can be used to
        /// serialize the message on another <see cref="SendContext" />.
        /// </summary>
        /// <param name="message">A message to overlay on top of the existing message, merging the properties together</param>
        /// <param name="messageTypes">The supported message types</param>
        /// <returns></returns>
        IMessageSerializer GetMessageSerializer(object message, string[] messageTypes);

        Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class;
    }
}
