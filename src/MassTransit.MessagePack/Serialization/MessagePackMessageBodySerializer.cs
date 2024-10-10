namespace MassTransit.Serialization;

using System;
using System.Net.Mime;
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
        return new MessagePackMessageBody<T>(context);
    }

    public void OverrideMessage<T>(T message)
        where T : class
    {
        _envelope.Message = MessagePackSerializer.Serialize(message, InternalMessagePackResolver.Options);
    }
}
