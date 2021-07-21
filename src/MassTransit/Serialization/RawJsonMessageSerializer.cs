namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Metadata;
    using Newtonsoft.Json;


    public class RawJsonMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/json";
        public static readonly ContentType RawJsonContentType = new ContentType(ContentTypeHeaderValue);

        readonly RawJsonSerializerOptions _options;

        public RawJsonMessageSerializer(RawJsonSerializerOptions options = RawJsonSerializerOptions.Default)
        {
            _options = options;
        }

        public static JsonSerializer Deserializer => JsonMessageSerializer.Deserializer;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = RawJsonContentType;

                if (_options.HasFlag(RawJsonSerializerOptions.AddTransportHeaders))
                    SetRawJsonMessageHeaders<T>(context);

                using var writer = new StreamWriter(stream, JsonMessageSerializer.Encoding, 1024, true);
                using var jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented};

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

        static void SetRawJsonMessageHeaders<T>(SendContext context)
            where T : class
        {
            if (context.MessageId.HasValue)
                context.Headers.Set(MessageHeaders.MessageId, context.MessageId.Value.ToString());

            if (context.CorrelationId.HasValue)
                context.Headers.Set(MessageHeaders.CorrelationId, context.CorrelationId.Value.ToString());

            if (context.ConversationId.HasValue)
                context.Headers.Set(MessageHeaders.ConversationId, context.ConversationId.Value.ToString());

            context.Headers.Set(MessageHeaders.MessageType, string.Join(";", TypeMetadataCache<T>.MessageTypeNames));

            if (context.ResponseAddress != null)
                context.Headers.Set(MessageHeaders.ResponseAddress, context.ResponseAddress);

            if (context.FaultAddress != null)
                context.Headers.Set(MessageHeaders.FaultAddress, context.FaultAddress);

            if (context.InitiatorId.HasValue)
                context.Headers.Set(MessageHeaders.InitiatorId, context.InitiatorId.Value.ToString());

            if (context.SourceAddress != null)
                context.Headers.Set(MessageHeaders.SourceAddress, context.SourceAddress);

            context.Headers.Set(MessageHeaders.ContentType, ContentTypeHeaderValue);
            context.Headers.Set(MessageHeaders.Host.Info, JsonConvert.SerializeObject(HostMetadataCache.Host, Formatting.None));
        }
    }
}
