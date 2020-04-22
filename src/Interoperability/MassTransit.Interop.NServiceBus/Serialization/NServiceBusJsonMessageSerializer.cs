namespace MassTransit.Interop.NServiceBus.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using MassTransit.Serialization;
    using Newtonsoft.Json;


    public class NServiceBusJsonMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/json";
        public static readonly ContentType JsonContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true),
            LazyThreadSafetyMode.PublicationOnly);

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = JsonContentType;

                context.SetNServiceBusHeaders();

                using var writer = new StreamWriter(stream, _encoding.Value, 1024, true);
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


        public ContentType ContentType => JsonContentType;
    }
}