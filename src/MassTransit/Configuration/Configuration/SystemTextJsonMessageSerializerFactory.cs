namespace MassTransit.Configuration
{
    using System;
    using System.Net.Mime;
    using Serialization;


    public class SystemTextJsonMessageSerializerFactory :
        ISerializerFactory
    {
        readonly Lazy<SystemTextJsonMessageSerializer> _serializer;

        public SystemTextJsonMessageSerializerFactory()
        {
            _serializer = new Lazy<SystemTextJsonMessageSerializer>(() => new SystemTextJsonMessageSerializer());
        }

        public ContentType ContentType => SystemTextJsonMessageSerializer.JsonContentType;

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
