#nullable enable
namespace MassTransit
{
    using System.Net.Mime;


    public interface ISerialization :
        IProbeSite
    {
        ContentType DefaultContentType { get; }

        IMessageSerializer GetMessageSerializer(ContentType? contentType = null);

        bool TryGetMessageSerializer(ContentType contentType, out IMessageSerializer serializer);

        IMessageDeserializer GetMessageDeserializer(ContentType? contentType = null);

        bool TryGetMessageDeserializer(ContentType contentType, out IMessageDeserializer deserializer);
    }
}
