namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Metadata;
    using Newtonsoft.Json;


    public class RawJsonMessageSerializer :
        RawMessageSerializer,
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/json";
        public static readonly ContentType RawJsonContentType = new ContentType(ContentTypeHeaderValue);

        readonly RawSerializerOptions _options;

        public RawJsonMessageSerializer(RawJsonSerializerOptions options = RawJsonSerializerOptions.Default)
        {
            _options = (RawSerializerOptions)options;
        }

        public static JsonSerializer Deserializer => JsonMessageSerializer.Deserializer;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = RawJsonContentType;

                if (_options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                    SetRawMessageHeaders<T>(context);

                using var writer = new StreamWriter(stream, JsonMessageSerializer.Encoding, 1024, true);
                using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented };

                JsonMessageSerializer.Serializer.Serialize(jsonWriter, context.Message, typeof(T));

                jsonWriter.Flush();
                writer.Flush();
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

        public ContentType ContentType => RawJsonContentType;

        protected override void SetRawMessageHeaders<T>(SendContext context)
            where T : class
        {
            base.SetRawMessageHeaders<T>(context);

            context.Headers.Set(MessageHeaders.ContentType, ContentTypeHeaderValue);
            context.Headers.Set(MessageHeaders.Host.Info, JsonConvert.SerializeObject(HostMetadataCache.Host, Formatting.None));
        }
    }
}
