namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using JsonConverters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;


    public class NewtonsoftJsonMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+json";
        public static readonly ContentType JsonContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<JsonSerializer> _serializer;

        public static readonly ByteArrayConverter ByteArrayConverter;
        public static readonly CaseInsensitiveDictionaryJsonConverter CaseInsensitiveDictionaryJsonConverter;
        public static readonly InterfaceProxyConverter InterfaceProxyConverter;
        public static readonly InternalTypeConverter InternalTypeConverter;
        public static readonly NewtonsoftMessageDataJsonConverter MessageDataJsonConverter;
        public static readonly StringDecimalConverter StringDecimalConverter;

        public static JsonSerializerSettings DeserializerSettings;
        public static JsonSerializerSettings SerializerSettings;

        static NewtonsoftJsonMessageSerializer()
        {
            GlobalTopology.MarkMessageTypeNotConsumable(typeof(JToken));

            ByteArrayConverter = new ByteArrayConverter();
            CaseInsensitiveDictionaryJsonConverter = new CaseInsensitiveDictionaryJsonConverter();
            InterfaceProxyConverter = new InterfaceProxyConverter();
            InternalTypeConverter = new InternalTypeConverter();
            MessageDataJsonConverter = new NewtonsoftMessageDataJsonConverter();
            StringDecimalConverter = new StringDecimalConverter();

            var namingStrategy = new CamelCaseNamingStrategy();

            DefaultContractResolver deserializerContractResolver = new JsonContractResolver(
                ByteArrayConverter,
                CaseInsensitiveDictionaryJsonConverter,
                InternalTypeConverter,
                InterfaceProxyConverter,
                MessageDataJsonConverter,
                StringDecimalConverter) { NamingStrategy = namingStrategy };

            DefaultContractResolver serializerContractResolver =
                new JsonContractResolver(ByteArrayConverter, MessageDataJsonConverter, StringDecimalConverter) { NamingStrategy = namingStrategy };

            DeserializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = deserializerContractResolver,
                TypeNameHandling = TypeNameHandling.None,
                DateParseHandling = DateParseHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            };

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

            _deserializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(DeserializerSettings));
            _serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(SerializerSettings));
        }

        public static JsonSerializer Deserializer => _deserializer.Value;

        public static JsonSerializer Serializer => _serializer.Value;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return new NewtonsoftJsonMessageBody<T>(context);
        }

        public ContentType ContentType => JsonContentType;
    }
}
