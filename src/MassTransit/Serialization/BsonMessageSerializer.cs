namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using Configuration;
    using JsonConverters;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public class BsonMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+bson";
        public static readonly ContentType BsonContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<JsonSerializer> _serializer;

        public static readonly ListJsonConverter ListJsonConverter;
        public static readonly CaseInsensitiveDictionaryJsonConverter CaseInsensitiveDictionaryJsonConverter;
        public static readonly InterfaceProxyConverter InterfaceProxyConverter;
        public static readonly MessageDataJsonConverter MessageDataJsonConverter;
        public static readonly IsoDateTimeConverter IsoDateTimeConverter;

        public static JsonSerializerSettings DeserializerSettings;
        public static JsonSerializerSettings SerializerSettings;

        static BsonMessageSerializer()
        {
            ListJsonConverter = new ListJsonConverter();
            CaseInsensitiveDictionaryJsonConverter = new CaseInsensitiveDictionaryJsonConverter();
            InterfaceProxyConverter = new InterfaceProxyConverter();
            MessageDataJsonConverter = new MessageDataJsonConverter();
            IsoDateTimeConverter = new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.RoundtripKind};

            var namingStrategy = new CamelCaseNamingStrategy();

            DefaultContractResolver deserializerContractResolver;
            if (AppContext.TryGetSwitch(AppContextSwitches.CaseSensitiveDictionaryDeserializer, out var isEnabled) && isEnabled)
            {
                deserializerContractResolver = new JsonContractResolver(
                    ListJsonConverter,
                    InterfaceProxyConverter,
                    IsoDateTimeConverter,
                    MessageDataJsonConverter) {NamingStrategy = namingStrategy};
            }
            else
            {
                deserializerContractResolver = new JsonContractResolver(
                    ListJsonConverter,
                    CaseInsensitiveDictionaryJsonConverter,
                    InterfaceProxyConverter,
                    IsoDateTimeConverter,
                    MessageDataJsonConverter) {NamingStrategy = namingStrategy};
            }

            IContractResolver serializerContractResolver =
                new JsonContractResolver(IsoDateTimeConverter, MessageDataJsonConverter) {NamingStrategy = namingStrategy};

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

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            context.ContentType = BsonContentType;

            var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

            using (var jsonWriter = new BsonDataWriter(stream))
            {
                _serializer.Value.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                jsonWriter.Flush();
            }
        }

        public ContentType ContentType => BsonContentType;
    }
}
