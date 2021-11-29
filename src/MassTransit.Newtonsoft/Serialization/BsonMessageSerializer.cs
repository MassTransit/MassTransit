namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using System.Net.Mime;
    using JsonConverters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public class BsonMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+bson";
        public static readonly ContentType BsonContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<JsonSerializer> _serializer;

        public static readonly CaseInsensitiveDictionaryJsonConverter CaseInsensitiveDictionaryJsonConverter;
        public static readonly InterfaceProxyConverter InterfaceProxyConverter;
        public static readonly InternalTypeConverter InternalTypeConverter;
        public static readonly NewtonsoftMessageDataJsonConverter MessageDataJsonConverter;
        public static readonly IsoDateTimeConverter IsoDateTimeConverter;

        public static JsonSerializerSettings DeserializerSettings;
        public static JsonSerializerSettings SerializerSettings;

        static readonly Lazy<IMessageSerializer> _instance = new Lazy<IMessageSerializer>(() => new BsonMessageSerializer());

        static BsonMessageSerializer()
        {
            CaseInsensitiveDictionaryJsonConverter = new CaseInsensitiveDictionaryJsonConverter();
            InterfaceProxyConverter = new InterfaceProxyConverter();
            InternalTypeConverter = new InternalTypeConverter();
            MessageDataJsonConverter = new NewtonsoftMessageDataJsonConverter();
            IsoDateTimeConverter = new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.RoundtripKind };

            var namingStrategy = new CamelCaseNamingStrategy();

            DefaultContractResolver deserializerContractResolver = new JsonContractResolver(
                CaseInsensitiveDictionaryJsonConverter,
                InternalTypeConverter,
                InterfaceProxyConverter,
                IsoDateTimeConverter,
                MessageDataJsonConverter) { NamingStrategy = namingStrategy };

            IContractResolver serializerContractResolver =
                new JsonContractResolver(IsoDateTimeConverter, MessageDataJsonConverter) { NamingStrategy = namingStrategy };

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

        public static IMessageSerializer Instance => _instance.Value;

        public static JsonSerializer Deserializer => _deserializer.Value;
        public static JsonSerializer Serializer => _serializer.Value;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return new NewtonsoftBsonMessageBody<T>(context);
        }

        public ContentType ContentType => BsonContentType;
    }
}
