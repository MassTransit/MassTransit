#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;


    public class Serialization :
        ISerialization
    {
        readonly IMessageDeserializer _defaultDeserializer;
        readonly IMessageSerializer _defaultSerializer;
        readonly IDictionary<string, IMessageDeserializer> _deserializers;
        readonly IDictionary<string, IMessageSerializer> _serializers;

        public Serialization(IEnumerable<IMessageSerializer> serializers, ContentType serializerContentType,
            IEnumerable<IMessageDeserializer> deserializers, ContentType defaultContentType)
        {
            if (serializerContentType == null)
                throw new ArgumentNullException(nameof(serializerContentType));
            if (defaultContentType == null)
                throw new ArgumentNullException(nameof(defaultContentType));

            DefaultContentType = defaultContentType;

            _serializers = new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase);

            foreach (var serializer in serializers)
                _serializers[serializer.ContentType.MediaType] = serializer;

            if (!_serializers.TryGetValue(serializerContentType.MediaType, out _defaultSerializer))
                throw new ConfigurationException($"The serializer content type was not found: {serializerContentType}");

            _deserializers = new Dictionary<string, IMessageDeserializer>(StringComparer.OrdinalIgnoreCase);

            foreach (var deserializer in deserializers)
                _deserializers[deserializer.ContentType.MediaType] = deserializer;

            if (!_deserializers.TryGetValue(defaultContentType.MediaType, out _defaultDeserializer))
                throw new ConfigurationException($"The default content type deserializer was not found: {defaultContentType}");
        }

        public ContentType DefaultContentType { get; }

        public IMessageSerializer GetMessageSerializer(ContentType? contentType = null)
        {
            var mediaType = contentType?.MediaType;

            if (mediaType != null && _serializers.TryGetValue(mediaType, out var serializer))
                return serializer;

            return _defaultSerializer;
        }

        public bool TryGetMessageSerializer(ContentType contentType, out IMessageSerializer serializer)
        {
            var mediaType = contentType.MediaType;

            return _serializers.TryGetValue(mediaType, out serializer);
        }

        public IMessageDeserializer GetMessageDeserializer(ContentType? contentType = null)
        {
            var mediaType = contentType?.MediaType;

            if (mediaType != null && _deserializers.TryGetValue(mediaType, out var deserializer))
                return deserializer;

            return _defaultDeserializer;
        }

        public bool TryGetMessageDeserializer(ContentType contentType, out IMessageDeserializer deserializer)
        {
            var mediaType = contentType.MediaType;

            return _deserializers.TryGetValue(mediaType, out deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("serializers");
            foreach (var deserializer in _deserializers.Values)
                deserializer.Probe(scope);
        }
    }
}
