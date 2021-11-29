namespace MassTransit.Configuration
{
    using System;
    using System.Net.Mime;
    using Serialization;


    public class SystemTextJsonRawMessageSerializerFactory :
        ISerializerFactory
    {
        readonly Lazy<SystemTextJsonRawMessageSerializer> _serializer;

        public SystemTextJsonRawMessageSerializerFactory(RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _serializer = new Lazy<SystemTextJsonRawMessageSerializer>(() => new SystemTextJsonRawMessageSerializer(options));
        }

        public ContentType ContentType => SystemTextJsonRawMessageSerializer.JsonContentType;

        public IMessageSerializer CreateSerializer()
        {
            return _serializer.Value;
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return _serializer.Value;
        }
    }
}
