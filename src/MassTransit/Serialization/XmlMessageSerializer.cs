namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public class XmlMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true),
            LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> _xmlSerializer;

        static readonly JsonSerializerSettings XmlSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new List<JsonConverter>(new[]
            {
                new XmlNodeConverter
                {
                    DeserializeRootElementName = "envelope",
                    WriteArrayAttribute = false,
                    OmitRootObject = true
                }
            })
        };

        static XmlMessageSerializer()
        {
            _xmlSerializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(XmlSerializerSettings));
        }

        public static JsonSerializer XmlSerializer => _xmlSerializer.Value;

        public ContentType ContentType => XmlContentType;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = XmlContentType;

                var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

                    XmlJsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                    stringWriter.Flush();
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var document = (XDocument)XmlSerializer.Deserialize(jsonReader, typeof(XDocument));

                    using (var writer = new StreamWriter(stream, _encoding.Value, 1024, true))
                    using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings {CheckCharacters = false}))
                    {
                        document.WriteTo(xmlWriter);
                    }
                }
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
    }
}
