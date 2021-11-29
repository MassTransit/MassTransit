namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Internals;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public static class SerializerCache
    {
        static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(CreateSerializer, LazyThreadSafetyMode.PublicationOnly);

        public static JsonSerializer Serializer => _serializer.Value;

        static JsonSerializer CreateSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.None,
                DateParseHandling = DateParseHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>(
                    new JsonConverter[] { new StringEnumConverter(), new InterfaceProxyConverter(TypeMetadataCache.ImplementationBuilder) })
            };

            return JsonSerializer.Create(settings);
        }


        class InterfaceProxyConverter :
            JsonConverter
        {
            readonly IImplementationBuilder _builder;

            public InterfaceProxyConverter(IImplementationBuilder builder)
            {
                if (builder == null)
                    throw new ArgumentNullException(nameof(builder));
                _builder = builder;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var proxyType = _builder.GetImplementationType(objectType);

                return serializer.Deserialize(reader, proxyType);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType.IsInterface;
            }
        }
    }
}
