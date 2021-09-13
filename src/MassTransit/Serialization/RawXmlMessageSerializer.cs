namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Metadata;
    using Newtonsoft.Json;


    public class RawXmlMessageSerializer :
        RawMessageSerializer,
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/xml";
        public static readonly ContentType RawXmlContentType = new ContentType(ContentTypeHeaderValue);

        readonly RawSerializerOptions _options;

        public RawXmlMessageSerializer(RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _options = options;
        }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = RawXmlContentType;

                if (_options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                    SetRawMessageHeaders<T>(context);

                XmlMessageSerializer.Serialize(stream, context.Message, typeof(T));
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

        public ContentType ContentType => RawXmlContentType;

        protected override void SetRawMessageHeaders<T>(SendContext context)
            where T : class
        {
            base.SetRawMessageHeaders<T>(context);

            context.Headers.Set(MessageHeaders.ContentType, ContentTypeHeaderValue);
            context.Headers.Set(MessageHeaders.Host.Info, JsonConvert.SerializeObject(HostMetadataCache.Host, Formatting.None));
        }
    }
}
