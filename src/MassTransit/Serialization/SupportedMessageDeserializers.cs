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
        readonly IDictionary<string, IMessageDeserializer> _deserializers;

        public SupportedMessageDeserializers(params IMessageDeserializer[] deserializers)
        {
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
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (string.IsNullOrWhiteSpace(contentType.MediaType))
                throw new ArgumentException("The media type must be specified", nameof(contentType));

            return _deserializers.TryGetValue(contentType.MediaType, out deserializer);
        }

        void AddSerializer(IMessageDeserializer deserializer)
        {
            if (deserializer == null)
                throw new ArgumentNullException(nameof(deserializer));

            string contentType = deserializer.ContentType.MediaType;

            _deserializers[contentType] = deserializer;
        }
    }
}
