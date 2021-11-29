namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using JsonConverters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public class NServiceBusXmlMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "text/xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

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
                    OmitRootObject = true
                }
            })
        };

        public static readonly Lazy<JsonSerializer> XmlSerializer;
        public static JsonSerializerSettings SerializerSettings;
        public static readonly Lazy<JsonSerializer> JsonSerializer;
        public static readonly ByteArrayConverter ByteArrayConverter;
        public static readonly NewtonsoftMessageDataJsonConverter MessageDataJsonConverter;
        public static readonly StringDecimalConverter StringDecimalConverter;

        static NServiceBusXmlMessageSerializer()
        {
            XmlSerializer = new Lazy<JsonSerializer>(() => Newtonsoft.Json.JsonSerializer.Create(XmlSerializerSettings));

            ByteArrayConverter = new ByteArrayConverter();
            MessageDataJsonConverter = new NewtonsoftMessageDataJsonConverter();
            StringDecimalConverter = new StringDecimalConverter();

            DefaultContractResolver serializerContractResolver =
                new JsonContractResolver(ByteArrayConverter, MessageDataJsonConverter, StringDecimalConverter) { NamingStrategy = new DefaultNamingStrategy() };

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

            JsonSerializer = new Lazy<JsonSerializer>(() => Newtonsoft.Json.JsonSerializer.Create(SerializerSettings));
        }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            context.SetNServiceBusHeaders();

            return new NServiceBusXmlMessageBody<T>(context);
        }

        public ContentType ContentType => XmlContentType;
    }
}
