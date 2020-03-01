namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GreenPipes;


    public class SupportedMessageDeserializers :
        IMessageDeserializer
    {
        readonly string _defaultContentType;
        readonly IDictionary<string, IMessageDeserializer> _deserializers;

        public SupportedMessageDeserializers(string defaultContentType, params IMessageDeserializer[] deserializers)
        {
            if (string.IsNullOrWhiteSpace(defaultContentType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(defaultContentType));

            _defaultContentType = defaultContentType;

            _deserializers = new Dictionary<string, IMessageDeserializer>(StringComparer.OrdinalIgnoreCase);

            foreach (IMessageDeserializer deserializer in deserializers)
                AddSerializer(deserializer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("deserializers");
            foreach (IMessageDeserializer deserializer in _deserializers.Values)
                deserializer.Probe(scope);
        }

        public ContentType ContentType { get; } = new ContentType("application/*");

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            if (!TryGetSerializer(receiveContext.ContentType, out var deserializer))
            {
                throw new SerializationException(
                    $"No deserializer was registered for the message content type: {receiveContext.ContentType}. Supported content types include {string.Join(", ", _deserializers.Values.Select(x => x.ContentType))}");
            }

            return deserializer.Deserialize(receiveContext);
        }

        bool TryGetSerializer(ContentType contentType, out IMessageDeserializer deserializer)
        {
            string mediaType = contentType?.MediaType ?? _defaultContentType;

            if (string.IsNullOrWhiteSpace(mediaType))
                throw new ArgumentException("The media type must be specified", nameof(contentType));

            return _deserializers.TryGetValue(mediaType, out deserializer);
        }

        void AddSerializer(IMessageDeserializer deserializer)
        {
            if (deserializer == null)
                throw new ArgumentNullException(nameof(deserializer));

            string mediaType = deserializer.ContentType.MediaType;

            _deserializers[mediaType] = deserializer;
        }
    }
}
