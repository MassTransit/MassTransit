namespace MassTransit.Serialization;

using System;
using System.Collections.Generic;
using System.Net.Mime;
using MassTransit.Internals.Json;
using MessagePack;

class MessagePackMessageBodySerializer :
    IMessageSerializer
{
    public ContentType ContentType { get; } = MessagePackMessageSerializer.MessagePackContentType;

    readonly MessagePackEnvelope _envelope;

    public MessagePackMessageBodySerializer(MessageEnvelope envelope)
    {
        _envelope = new MessagePackEnvelope(envelope);
    }

    public MessageBody GetMessageBody<T>(SendContext<T> context)
        where T : class
    {
        _envelope.Update(context);

        if (_envelope.MessageType != null)
        {
            context.SupportedMessageTypes = _envelope.MessageType;
        }

        return new MessagePackMessageBody<T>(context, _envelope);
    }

    public void OverrideMessage<T>(T message)
        where T : class
    {
        var currentMessage = MessagePackSerializer
            .Deserialize<Dictionary<string, object>>((byte[])_envelope.Message);

        currentMessage = new Dictionary<string, object>(currentMessage, StringComparer.OrdinalIgnoreCase);

        var messageToMerge = message
            .Transform<Dictionary<string, object>>(SystemTextJsonMessageSerializer.Options);

        foreach (var overlay in messageToMerge)
        {
            currentMessage[overlay.Key] = overlay.Value;
        }


        _envelope.IsMessageNativeMessagePackSerialized = false;
        _envelope.Message = MessagePackSerializer
            .Serialize(currentMessage, InternalMessagePackResolver.Options);
    }
}
