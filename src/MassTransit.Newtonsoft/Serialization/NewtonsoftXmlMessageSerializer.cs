namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public class NewtonsoftXmlMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

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

        static NewtonsoftXmlMessageSerializer()
        {
            _xmlSerializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(XmlSerializerSettings));
        }

        public static JsonSerializer XmlSerializer => _xmlSerializer.Value;

        public ContentType ContentType => XmlContentType;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return new NewtonsoftXmlMessageBody<T>(context);
        }

        public static void Serialize(Stream stream, object message, Type messageType)
        {
            var json = new StringBuilder(1024);

            using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
            using (var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Newtonsoft.Json.Formatting.None })
            {
                NewtonsoftXmlJsonMessageSerializer.Serializer.Serialize(jsonWriter, message, messageType);

                jsonWriter.Flush();
                stringWriter.Flush();
            }

            using (var stringReader = new StringReader(json.ToString()))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var document = (XDocument)XmlSerializer.Deserialize(jsonReader, typeof(XDocument));

                using (var writer = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true))
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { CheckCharacters = false }))
                {
                    document.WriteTo(xmlWriter);
                }
            }
        }
    }
}
