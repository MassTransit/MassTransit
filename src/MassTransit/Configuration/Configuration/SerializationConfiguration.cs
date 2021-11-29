#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Serialization;
    using Util;


    public class SerializationConfiguration :
        ISerializationConfiguration
    {
        readonly Lazy<ISerialization> _collection;
        readonly IDictionary<string, ISerializerFactory> _deserializers;
        readonly IDictionary<string, ISerializerFactory> _serializers;
        ContentType? _defaultContentType;
        ContentType? _serializerContentType;
        SerializationConfiguration? _source;

        public SerializationConfiguration()
        {
            _serializers = new Dictionary<string, ISerializerFactory>(StringComparer.OrdinalIgnoreCase);
            _deserializers = new Dictionary<string, ISerializerFactory>(StringComparer.OrdinalIgnoreCase);
            _collection = new Lazy<ISerialization>(() => CreateCollection());

            AddSystemTextJson();
        }

        SerializationConfiguration(SerializationConfiguration source)
        {
            _serializers = new Dictionary<string, ISerializerFactory>(StringComparer.OrdinalIgnoreCase);
            _deserializers = new Dictionary<string, ISerializerFactory>(StringComparer.OrdinalIgnoreCase);
            _collection = new Lazy<ISerialization>(() => CreateCollection());

            _source = source;
        }

        public ContentType DefaultContentType
        {
            set
            {
                if (_collection.IsValueCreated)
                    throw new ConfigurationException("The serializer collection was already created.");

                _defaultContentType = value;
            }
        }

        public ContentType SerializerContentType
        {
            set
            {
                if (_collection.IsValueCreated)
                    throw new ConfigurationException("The serializer collection was already created.");

                _serializerContentType = value;
            }
        }

        public void Clear()
        {
            _serializers.Clear();
            _deserializers.Clear();

            _defaultContentType = default;
            _serializerContentType = default;

            _source = null;
        }

        public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
        {
            if (_collection.IsValueCreated)
                throw new ConfigurationException("The serializer collection was already created.");

            _serializers[factory.ContentType.MediaType] = factory;

            if (isSerializer)
                SerializerContentType = factory.ContentType;
        }

        public void AddDeserializer(ISerializerFactory factory, bool isDefault = false)
        {
            if (_collection.IsValueCreated)
                throw new ConfigurationException("The serializer collection was already created.");

            _deserializers[factory.ContentType.MediaType] = factory;

            if (isDefault)
                DefaultContentType = factory.ContentType;
        }

        public ISerializationConfiguration CreateSerializationConfiguration()
        {
            return new SerializationConfiguration(this);
        }

        public ISerialization CreateSerializerCollection()
        {
            return _collection.Value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            IEnumerable<ISerializerFactory> serializers = _serializers.Values;
            if (_source != null)
            {
                serializers = serializers.Concat(_source._serializers.Values)
                    .Distinct((x, y) => x.ContentType.MediaType.Equals(y.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));
            }

            var mediaTypes = serializers.Select(x => x.ContentType.MediaType).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (mediaTypes.Length == 0)
                yield return this.Failure("Serializers", "must specify at least one serializer");

            var serializerMediaType = _serializerContentType?.MediaType ?? _source?._serializerContentType?.MediaType
                ?? (mediaTypes.Length == 1 ? mediaTypes[0] : default)
                ?? throw new ConfigurationException("No serializer content type specified and more than one serializer was configured");

            if (string.IsNullOrWhiteSpace(serializerMediaType))
            {
                if (mediaTypes.Length > 1)
                    yield return this.Failure("SerializerContentType", "must be specified when more than one serializer is supported");

                if (!mediaTypes.Any(x => x.Equals(serializerMediaType, StringComparison.OrdinalIgnoreCase)))
                    yield return this.Failure("SerializerContentType", "matching serializer was not added");
            }

            IEnumerable<ISerializerFactory> deserializers = _deserializers.Values;
            if (_source != null)
            {
                deserializers = deserializers.Concat(_source._deserializers.Values)
                    .Distinct((x, y) => x.ContentType.MediaType.Equals(y.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));
            }

            mediaTypes = deserializers.Select(x => x.ContentType.MediaType).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (mediaTypes.Length == 0)
                yield return this.Failure("Deserializers", "must specify at least one deserializer");

            var defaultMediaType = _defaultContentType?.MediaType ?? _source?._defaultContentType?.MediaType
                ?? (mediaTypes.Length == 1 ? mediaTypes[0] : default)
                ?? throw new ConfigurationException("No default content type specified and more than one deserializer was configured");

            if (string.IsNullOrWhiteSpace(defaultMediaType))
            {
                if (mediaTypes.Length > 1)
                    yield return this.Failure("DefaultContentType", "must be specified when more than one deserializer is supported");

                if (!mediaTypes.Any(x => x.Equals(defaultMediaType, StringComparison.OrdinalIgnoreCase)))
                    yield return this.Failure("DefaultContentType", "matching deserializer was not added");
            }
        }

        ISerialization CreateCollection()
        {
            IEnumerable<ISerializerFactory> serializers = _serializers.Values;
            if (_source != null)
            {
                serializers = serializers.Concat(_source._serializers.Values)
                    .Distinct((x, y) => x.ContentType.MediaType.Equals(y.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));
            }

            IMessageSerializer[] messageSerializers = serializers.Select(x => x.CreateSerializer()).ToArray();

            var serializerContentType = _serializerContentType ?? _source?._serializerContentType
                ?? (messageSerializers.Length == 1 ? messageSerializers[0].ContentType : default)
                ?? throw new ConfigurationException("No serializer content type specified and more than one serializer was configured");

            IEnumerable<ISerializerFactory> deserializers = _deserializers.Values;
            if (_source != null)
            {
                deserializers = deserializers.Concat(_source._deserializers.Values)
                    .Distinct((x, y) => x.ContentType.MediaType.Equals(y.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));
            }

            IMessageDeserializer[] messageDeserializers = deserializers.Select(x => x.CreateDeserializer()).ToArray();

            var defaultContentType = _defaultContentType ?? _source?._defaultContentType
                ?? (messageDeserializers.Length == 1 ? messageDeserializers[0].ContentType : default)
                ?? throw new ConfigurationException("No default content type specified and more than one deserializer was configured");

            return new Serialization(messageSerializers, serializerContentType, messageDeserializers, defaultContentType);
        }

        void AddSystemTextJson()
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            AddSerializer(factory);
            AddDeserializer(factory, true);
        }
    }
}
