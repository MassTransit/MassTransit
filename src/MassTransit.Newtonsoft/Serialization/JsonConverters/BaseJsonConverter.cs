namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;


    public abstract class BaseJsonConverter :
        JsonConverter
    {
        readonly ConcurrentDictionary<Type, IConverter> _cached = new ConcurrentDictionary<Type, IConverter>();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _cached.GetOrAdd(objectType, CreateMissingConverter).Deserialize(reader, objectType, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return _cached.GetOrAdd(objectType, CreateMissingConverter).IsSupported;
        }

        IConverter CreateMissingConverter(Type objectType)
        {
            return ValueFactory(objectType);
        }

        protected abstract IConverter ValueFactory(Type objectType);


        protected class Unsupported :
            IConverter
        {
            public object Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                throw new MassTransitException($"Unsupported deserialization object type: {TypeCache.GetShortName(objectType)}");
            }

            public bool IsSupported => false;
        }


        protected interface IConverter
        {
            bool IsSupported { get; }

            object Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer);
        }
    }
}
