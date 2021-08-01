namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using GreenPipes;
    using Serialization;


    public class SerializationConfiguration :
        ISerializationConfiguration
    {
        readonly Lazy<IMessageDeserializer> _deserializer;
        readonly IDictionary<string, DeserializerFactory> _deserializerFactories;
        readonly Lazy<IMessageSerializer> _serializer;
        string _defaultContentType;
        SerializerFactory _serializerFactory;
        IDictionary<string, DeserializerFactory> _sourceDeserializerFactories;

        public SerializationConfiguration()
        {
            _serializerFactory = () => new JsonMessageSerializer();
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);

            _defaultContentType = JsonMessageSerializer.JsonContentType.MediaType;

            _deserializerFactories = new Dictionary<string, DeserializerFactory>(StringComparer.OrdinalIgnoreCase);

            AddDeserializer(JsonMessageSerializer.JsonContentType, () => new JsonMessageDeserializer(JsonMessageSerializer.Deserializer));
            AddDeserializer(SystemTextJsonMessageSerializer.JsonContentType, () => new SystemTextJsonMessageDeserializer());
            AddDeserializer(BsonMessageSerializer.BsonContentType, () => new BsonMessageDeserializer(BsonMessageSerializer.Deserializer));
            AddDeserializer(XmlMessageSerializer.XmlContentType, () => new XmlMessageDeserializer(XmlJsonMessageSerializer.Deserializer));
        }

        SerializationConfiguration(SerializationConfiguration source)
        {
            _serializerFactory = () => source.Serializer;
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);

            _defaultContentType = source._defaultContentType;

            _sourceDeserializerFactories = source._deserializerFactories;
            _deserializerFactories = new Dictionary<string, DeserializerFactory>(StringComparer.OrdinalIgnoreCase);
        }

        public IMessageSerializer Serializer => _serializer.Value;
        public IMessageDeserializer Deserializer => _deserializer.Value;

        public void AddDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            if (deserializerFactory == null)
                throw new ArgumentNullException(nameof(deserializerFactory));

            _deserializerFactories[contentType.MediaType] = deserializerFactory;
        }

        public void SetSerializer(SerializerFactory serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException(nameof(serializerFactory));

            if (_serializer.IsValueCreated)
                throw new ConfigurationException("The serializer has already been created, the serializer cannot be changed at this time.");

            _serializerFactory = serializerFactory;
        }

        public void ClearDeserializers()
        {
            _deserializerFactories.Clear();
            _sourceDeserializerFactories = null;
            _defaultContentType = default;
        }

        public ContentType DefaultContentType
        {
            set => _defaultContentType = value?.MediaType;
        }

        public ISerializationConfiguration CreateSerializationConfiguration()
        {
            return new SerializationConfiguration(this);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            IEnumerable<KeyValuePair<string, DeserializerFactory>> deserializerFactories = _sourceDeserializerFactories != null
                ? deserializerFactories = _deserializerFactories.Concat(_sourceDeserializerFactories)
                : _deserializerFactories;

            List<string> mediaTypes = deserializerFactories.Select(x => x.Key).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (mediaTypes.Count == 0)
                yield return this.Failure("Deserializers", "must specify at least one deserializer");

            if (string.IsNullOrWhiteSpace(_defaultContentType) && mediaTypes.Count > 1)
                yield return this.Failure("DefaultContentType", "must be specified when more than one deserializer is supported");
        }

        IMessageSerializer CreateSerializer()
        {
            return _serializerFactory();
        }

        IMessageDeserializer CreateDeserializer()
        {
            if (_sourceDeserializerFactories != null)
            {
                foreach (KeyValuePair<string, DeserializerFactory> deserializerFactory in _sourceDeserializerFactories)
                {
                    if (!_deserializerFactories.ContainsKey(deserializerFactory.Key))
                        _deserializerFactories.Add(deserializerFactory);
                }
            }

            IMessageDeserializer[] deserializers = _deserializerFactories.Values.Select(x => x()).ToArray();

            if (string.IsNullOrWhiteSpace(_defaultContentType) && _deserializerFactories.Count == 1)
                _defaultContentType = _deserializerFactories.Single().Key;

            return new SupportedMessageDeserializers(_defaultContentType, deserializers);
        }
    }
}
