namespace MassTransit.GrpcTransport.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class GrpcMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/bson";
        public static readonly ContentType GrpcContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<Encoding> _encoding;
        static readonly Lazy<JsonSerializer> _serializer;

        static GrpcMessageSerializer()
        {
            _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true), LazyThreadSafetyMode.PublicationOnly);

            _deserializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(BsonMessageSerializer.DeserializerSettings));
            _serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(BsonMessageSerializer.SerializerSettings));
        }

        public static JsonSerializer Deserializer => _deserializer.Value;
        public static JsonSerializer Serializer => _serializer.Value;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = GrpcContentType;

                using var jsonWriter = new BsonDataWriter(stream);

                _serializer.Value.Serialize(jsonWriter, context.Message, typeof(T));

                jsonWriter.Flush();
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public ContentType ContentType => GrpcContentType;
    }
}
