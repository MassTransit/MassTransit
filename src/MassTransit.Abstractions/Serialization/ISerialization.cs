namespace MassTransit
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;


    public interface ISerialization :
        IProbeSite
    {
        ContentType DefaultContentType { get; }

        IMessageSerializer GetMessageSerializer(ContentType? contentType = null);

        bool TryGetMessageSerializer(ContentType contentType, [NotNullWhen(true)] out IMessageSerializer? serializer);

        IMessageDeserializer GetMessageDeserializer(ContentType? contentType = null);

        bool TryGetMessageDeserializer(ContentType contentType, [NotNullWhen(true)] out IMessageDeserializer? deserializer);
    }
}
