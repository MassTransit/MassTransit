namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using JsonConverters;
    using Metadata;


    public class SystemTextJsonMessageSerializer :
        IMessageSerializer
    {
        public static readonly ContentType JsonContentType = new ContentType("application/vnd.masstransit+text-json");

        public static JsonSerializerOptions Options;

        static SystemTextJsonMessageSerializer()
        {
            Options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
            };

            Options.Converters.Add(new SystemTextJsonTimeSpanConverter());
            Options.Converters.Add(new SystemTextJsonMessageDataConverter());
            Options.Converters.Add(new SystemTextJsonConverterFactory());
        }

        public SystemTextJsonMessageSerializer(ContentType contentType = null)
        {
            ContentType = contentType ?? JsonContentType;
        }

        public ContentType ContentType { get; }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = JsonContentType;

                var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

                using var writer = new Utf8JsonWriter(stream);

                JsonSerializer.Serialize(writer, envelope, Options);

                writer.Flush();
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}
