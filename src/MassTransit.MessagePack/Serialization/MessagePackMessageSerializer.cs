namespace MassTransit.Serialization;

using System;
using System.Collections.Generic;
using System.Net.Mime;
using Initializers;
using Initializers.TypeConverters;
using Internals;
using MessagePack;


public class MessagePackMessageSerializer :
    IMessageSerializer,
    IMessageDeserializer,
    IObjectDeserializer
{
    const string ContentTypeHeaderValue = "application/vnd.masstransit+msgpack";
    const string ProviderKey = "MessagePack";

    public static readonly ContentType MessagePackContentType = new(ContentTypeHeaderValue);

    public ContentType ContentType => MessagePackContentType;

    public ConsumeContext Deserialize(ReceiveContext receiveContext)
    {
        var serializerContext = Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress);
        return new BodyConsumeContext(receiveContext, serializerContext);
    }

    public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
    {
        var messageBuffer = body.GetBytes();
        var envelope = DeserializeMessageBuffer<MessagePackEnvelope>(messageBuffer);

        var messageContext = new EnvelopeMessageContext(envelope, this);

        var messageTypes = envelope.MessageType ?? [];

        return new MessagePackMessageSerializerContext(this, messageContext, messageTypes, envelope);
    }

    public MessageBody GetMessageBody(string text)
    {
        return new Base64MessageBody(text);
    }

    public MessageBody GetMessageBody<T>(SendContext<T> context)
        where T : class
    {
        return new MessagePackMessageBody<T>(context);
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateScope("messagepack");
        scope.Add("contentType", ContentType.MediaType);
        scope.Add("provider", ProviderKey);
    }

    public static byte[] EnsureObjectBufferFormatIsByteArray(object serializedObjectAsUnknownFormat)
    {
        return serializedObjectAsUnknownFormat switch
        {
            string base64EncodedMessagePackBody => Convert.FromBase64String(base64EncodedMessagePackBody),
            byte[] messagePackBody => messagePackBody,
            _ => MessagePackSerializer.Serialize(serializedObjectAsUnknownFormat, InternalMessagePackResolver.Options)
        };
    }

    public T? DeserializeObject<T>(object? value, T? defaultValue = default)
        where T : class
    {
        if (value is Dictionary<string, object> objectByStringPairs)
        {
            // If the object is a Dictionary<string, object>, we deserialize internally using JSON.
            // MessagePack is case-sensitive, and would not be able to deserialize without correct casing.

            return objectByStringPairs.Transform<T>(SystemTextJsonMessageSerializer.Options);
        }

        return InternalDeserializeObject(value, defaultValue);
    }

    public T? DeserializeObject<T>(object? value, T? defaultValue = null)
        where T : struct
    {
        return InternalDeserializeObject(value, defaultValue);
    }

    public MessageBody SerializeObject(object? value)
    {
        if (value is null)
            return new EmptyMessageBody();

        return new MessagePackMessageBody<object>(value);
    }

    static T InternalDeserializeObject<T>(object? value, T defaultValue)
    {
        if (value is null || Equals(value, defaultValue))
            return defaultValue;

        if (value is T valueAsT)
            return valueAsT;

        if (value is string text
            && TypeConverterCache.TryGetTypeConverter<T, string>(out ITypeConverter<T, string>? typeConverter)
            && typeConverter.TryConvert(text, out var result))
            return result;

        var messageSerializedBuffer = EnsureObjectBufferFormatIsByteArray(value);

        return DeserializeMessageBuffer<T>(messageSerializedBuffer);
    }

    static T DeserializeMessageBuffer<T>(byte[] messageBuffer)
    {
        return MessagePackSerializer.Deserialize<T>(messageBuffer, InternalMessagePackResolver.Options);
    }
}
