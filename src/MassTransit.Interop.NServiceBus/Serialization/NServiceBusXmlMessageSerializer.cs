namespace MassTransit.Interop.NServiceBus.Serialization
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
    using MassTransit.Serialization.JsonConverters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public class NServiceBusXmlMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "text/xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<Encoding> Encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true),
            LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> XmlSerializer;

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
                    DeserializeRootElementName = "message",
                    WriteArrayAttribute = false,
                    OmitRootObject = true,
                }
            })
        };


        public static JsonSerializerSettings SerializerSettings;
        static readonly Lazy<JsonSerializer> _serializer;
        public static readonly ByteArrayConverter ByteArrayConverter;
        public static readonly MessageDataJsonConverter MessageDataJsonConverter;
        public static readonly StringDecimalConverter StringDecimalConverter;

        static NServiceBusXmlMessageSerializer()
        {
            XmlSerializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(XmlSerializerSettings));

            ByteArrayConverter = new ByteArrayConverter();
            MessageDataJsonConverter = new MessageDataJsonConverter();
            StringDecimalConverter = new StringDecimalConverter();

            DefaultContractResolver serializerContractResolver =
                new JsonContractResolver(ByteArrayConverter, MessageDataJsonConverter, StringDecimalConverter) {NamingStrategy = new DefaultNamingStrategy()};

            SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = serializerContractResolver,
                TypeNameHandling = TypeNameHandling.None,
                DateParseHandling = DateParseHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            };

            _serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(SerializerSettings));
        }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = XmlContentType;

                context.SetNServiceBusHeaders();

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter) {Formatting = Newtonsoft.Json.Formatting.None})
                {
                    _serializer.Value.Serialize(jsonWriter, context.Message, typeof(T));

                    jsonWriter.Flush();
                    stringWriter.Flush();
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var document = (XDocument) XmlSerializer.Value.Deserialize(jsonReader, typeof(XDocument));

                    if (document.Root != null)
                        document.Root.Name = typeof(T).Name;

                    using (var writer = new StreamWriter(stream, Encoding.Value, 1024, true))
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


        public ContentType ContentType => XmlContentType;
    }
}