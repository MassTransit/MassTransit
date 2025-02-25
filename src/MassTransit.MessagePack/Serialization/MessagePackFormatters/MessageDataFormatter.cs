namespace MassTransit.Serialization.MessagePackFormatters;

using JsonConverters;
using MessageData.Values;
using MessagePack;
using MessagePack.Formatters;


public class MessageDataFormatter<T> :
    IMessagePackFormatter<MessageData<T>?>
{
    public void Serialize(ref MessagePackWriter writer, MessageData<T>? value, MessagePackSerializerOptions options)
    {
        var reference = new SystemTextMessageDataReference { Reference = value?.Address };

        // Borrows System.Text.Json's SystemTextMessageDataReference type.
        IMessagePackFormatter<SystemTextMessageDataReference> innerFormatter = options.Resolver.GetFormatterWithVerify<SystemTextMessageDataReference>();

        innerFormatter.Serialize(ref writer, reference, options);
    }

    public MessageData<T>? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        IMessagePackFormatter<SystemTextMessageDataReference> innerFormatter = options.Resolver.GetFormatterWithVerify<SystemTextMessageDataReference>();

        var reference = innerFormatter.Deserialize(ref reader, options);

        if (reference?.Text != null)
            return (MessageData<T>?)new StringInlineMessageData(reference.Text, reference.Reference);
        if (reference?.Data != null)
            return (MessageData<T>?)new BytesInlineMessageData(reference.Data, reference.Reference);

        if (reference?.Reference == null)
            return EmptyMessageData<T>.Instance;

        return new DeserializedMessageData<T>(reference.Reference);
    }
}
