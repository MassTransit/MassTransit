namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using JsonConverters;
    using Newtonsoft.Json;


    public class NewtonsoftXmlJsonMessageSerializer
    {
        static readonly Lazy<JsonSerializer> _deserializer;
        static readonly Lazy<JsonSerializer> _serializer;

        static readonly ListJsonConverter ListJsonConverter;

        static NewtonsoftXmlJsonMessageSerializer()
        {
            ListJsonConverter = new ListJsonConverter();

            _deserializer = new Lazy<JsonSerializer>(() => CreateDeserializer());
            _serializer = new Lazy<JsonSerializer>(() => CreateSerializer());
        }

        public static JsonSerializer Deserializer => _deserializer.Value;

        public static JsonSerializer Serializer => _serializer.Value;

        static JsonSerializer CreateSerializer()
        {
            var source = NewtonsoftJsonMessageSerializer.SerializerSettings;

            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = source.NullValueHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                TypeNameHandling = source.TypeNameHandling,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            return JsonSerializer.Create(serializerSettings);
        }

        static JsonSerializer CreateDeserializer()
        {
            var source = NewtonsoftJsonMessageSerializer.DeserializerSettings;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = source.NullValueHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                TypeNameHandling = source.TypeNameHandling,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters) {ListJsonConverter}
            };

            return JsonSerializer.Create(settings);
        }
    }
}
