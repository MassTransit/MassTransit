using System;
using System.Collections.Generic;

namespace MassTransit.Serialization
{
    public static class MessageSerializerRepository
    {
        static readonly Dictionary<string, IMessageSerializer> Serializers;
        static IMessageSerializer _defaultSerializer;

        static MessageSerializerRepository()
        {
            Serializers = new Dictionary<string, IMessageSerializer>();

            RegisterSerializers();
        }

        public static void RegisterSerializer(IMessageSerializer serializer)
        {
            if (Serializers.ContainsKey(serializer.ContentType))
            {
                throw new ArgumentException("A serializer with the specifiec ContentType is already registered", "serializer");
            }

            Serializers.Add(serializer.ContentType, serializer);
        }

        public static void RegisterDefaultSerializer(IMessageSerializer serializer)
        {
            _defaultSerializer = serializer;
            Serializers[serializer.ContentType] = serializer;
        }

        public static IMessageSerializer LookupSerializer(string contentType)
        {
            IMessageSerializer serializer;
            return Serializers.TryGetValue(contentType, out serializer)
                       ? serializer
                       : _defaultSerializer;
        }

        private static void RegisterSerializers()
        {
            RegisterSerializer(new BinaryMessageSerializer());
            RegisterSerializer(new BsonMessageSerializer());
            RegisterSerializer(new JsonMessageSerializer());
            RegisterSerializer(new XmlMessageSerializer());

            RegisterDefaultSerializer(new VersionOneXmlMessageSerializer());
        }
    }
}
