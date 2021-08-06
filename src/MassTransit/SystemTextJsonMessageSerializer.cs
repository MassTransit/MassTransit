namespace MassTransit.Serialization
{
    using MassTransit.Metadata;
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Json;

    public class SystemTextJsonMessageSerializer : IMessageSerializer
    {
        public ContentType ContentType => JsonContentType;

        internal static readonly ContentType JsonContentType = new ContentType("application/vnd.masstransit+json2");

        public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
        {
            try
            {
                context.ContentType = JsonContentType;

                var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

                using (var ms = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes<MessageEnvelope>(envelope, SystemTextJsonConfiguration.Options)))
                {
                    ms.CopyTo(stream);
                }

            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}
