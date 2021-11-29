namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class GrpcMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/bson";
        public static readonly ContentType GrpcContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<JsonSerializer> _serializer;

        static GrpcMessageSerializer()
        {
            _deserializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(BsonMessageSerializer.DeserializerSettings));
            _serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(BsonMessageSerializer.SerializerSettings));
        }

        public static JsonSerializer Deserializer => _deserializer.Value;
        public static JsonSerializer Serializer => _serializer.Value;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return new NewtonsoftRawBsonMessageBody<T>(context);
        }

        public ContentType ContentType => GrpcContentType;
    }
}
