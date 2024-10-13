namespace MassTransit.Serialization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Internals;
using MessagePack;


public class MessagePackMessageSerializerContext :
    BaseSerializerContext
{
    readonly MessagePackEnvelope _envelope;

    public MessagePackMessageSerializerContext(MessagePackMessageSerializer serializer, MessageContext context, string[] supportedMessageTypes,
        MessagePackEnvelope envelope)
        : base(serializer, context, supportedMessageTypes)
    {
        _envelope = envelope;

        if (_envelope.Message is null)
            throw new ArgumentException("Message cannot be null.", nameof(envelope));
    }

    public override bool TryGetMessage<T>(out T? message)
        where T : class
    {
        if (!TryGetMessage(typeof(T), out var outMessage))
        {
            message = default;
            return false;
        }

        message = (T)outMessage!;
        return true;
    }

    public override bool TryGetMessage(Type messageType, [NotNullWhen(true)] out object? message)
    {
        try
        {
            if (!IsSupportedMessageType(messageType))
            {
                message = null;
                return false;
            }

            var messagePackSerializedObjectBuffer = MessagePackMessageSerializer.EnsureObjectBufferFormatIsByteArray(_envelope.Message!);

            if (_envelope.IsMessageNativeMessagePackSerialized)
                message = MessagePackSerializer.Deserialize(messageType, messagePackSerializedObjectBuffer, InternalMessagePackResolver.Options);
            else
            {
                // If a message is serialized as dictionary of string-object pairs, we need to deserialize using a different approach.

                var messageAsDictionary = MessagePackSerializer
                    .Deserialize<Dictionary<string, object>>(messagePackSerializedObjectBuffer, InternalMessagePackResolver.Options);

                message = messageAsDictionary.Transform(messageType, SystemTextJsonMessageSerializer.Options);
            }

            return message != default;
        }
        catch
        {
            message = default;
            return false;
        }
    }

    public override IMessageSerializer GetMessageSerializer()
    {
        if (_envelope is null)
            throw new InvalidOperationException("Context has no envelope.");

        return new MessagePackMessageBodySerializer(_envelope);
    }

    public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
    {
        var messageEnvelopeSerializer = new MessagePackMessageBodySerializer(envelope);

        messageEnvelopeSerializer.OverrideMessage(message);

        return messageEnvelopeSerializer;
    }

    public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
    {
        var messagePackEnvelope = new MessagePackEnvelope(this, message, messageTypes);

        return new MessagePackMessageBodySerializer(messagePackEnvelope);
    }

    public override Dictionary<string, object> ToDictionary<T>(T? message)
        where T : class
    {
        if (message is null)
            return new Dictionary<string, object>(0, StringComparer.OrdinalIgnoreCase);

        // We serialize internally using JSON.
        return message.Transform<Dictionary<string, object>>(SystemTextJsonMessageSerializer.Options)
            ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
