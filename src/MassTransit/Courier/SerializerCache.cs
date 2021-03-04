namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;


    public static class SerializerCache
    {
        static readonly Lazy<JsonSerializer> _deserializer = new Lazy<JsonSerializer>(CreateDeserializer, LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(CreateSerializer, LazyThreadSafetyMode.PublicationOnly);
        static JsonSerializerSettings _serializerSettings;

        public static JsonSerializer Serializer => _serializer.Value;

        public static JsonSerializer Deserializer => _deserializer.Value;

        static JsonSerializer CreateSerializer()
        {
            var source = JsonMessageSerializer.SerializerSettings;

            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                TypeNameHandling = source.TypeNameHandling,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            _serializerSettings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(_serializerSettings);
        }

        static JsonSerializer CreateDeserializer()
        {
            var source = JsonMessageSerializer.DeserializerSettings;

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
                Converters = new List<JsonConverter>(source.Converters)
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(settings);
        }

        public static IDictionary<string, object> GetObjectAsDictionary(object values)
        {
            if (values == null)
                return new Dictionary<string, object>();

            var dictionary = JObject.FromObject(values, Serializer);

            return dictionary.ToObject<IDictionary<string, object>>();
        }

        public static string ConvertDictionaryToString(IDictionary<string, object> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary, Formatting.None, _serializerSettings);
        }

        public static IDictionary<string, object> ConvertStringToDictionary(string json)
        {
            return JObject.Parse(json).ToObject<IDictionary<string, object>>();
        }
    }
}
