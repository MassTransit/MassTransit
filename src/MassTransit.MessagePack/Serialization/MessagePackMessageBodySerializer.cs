namespace MassTransit.Serialization;

using System;
using System.Collections.Generic;
using System.Net.Mime;
using Internals;
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
            context.SupportedMessageTypes = _envelope.MessageType;

        return new MessagePackMessageBody<T>(context, _envelope);
    }

    public void OverrideMessage<T>(T message)
        where T : class
    {
        Dictionary<string, object> currentMessage;

        if (_envelope.Message is not null)
        {
            currentMessage = MessagePackSerializer
                .Deserialize<Dictionary<string, object>>((byte[])_envelope.Message);
        }
        else
            currentMessage = new Dictionary<string, object>(0);

        currentMessage = new Dictionary<string, object>(currentMessage, StringComparer.OrdinalIgnoreCase);
        var messageToMerge = message
            .Transform<Dictionary<string, object>>(SystemTextJsonMessageSerializer.Options);

        if (messageToMerge is null)
        {
            // If we are unable to transform the message,
            // we should not override the message.
            return;
        }

        foreach (KeyValuePair<string, object> overlay in messageToMerge)
            currentMessage[overlay.Key] = overlay.Value;


        _envelope.IsMessageNativeMessagePackSerialized = false;
        _envelope.Message = MessagePackSerializer
            .Serialize(currentMessage, InternalMessagePackResolver.Options);
    }
}
