namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public static class SerializerCache
    {
        static readonly Lazy<JsonSerializer> _deserializer = new Lazy<JsonSerializer>(CreateDeserializer, LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(CreateSerializer, LazyThreadSafetyMode.PublicationOnly);

        public static JsonSerializer Serializer => _serializer.Value;

        public static JsonSerializer Deserializer => _deserializer.Value;

        static JsonSerializer CreateSerializer()
        {
            JsonSerializerSettings source = JsonMessageSerializer.SerializerSettings;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(settings);
        }

        static JsonSerializer CreateDeserializer()
        {
            JsonSerializerSettings source = JsonMessageSerializer.DeserializerSettings;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = source.NullValueHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(settings);
        }
    }
}
