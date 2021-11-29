namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class GrpcSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => GrpcMessageSerializer.GrpcContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new GrpcMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new GrpcMessageDeserializer(GrpcMessageSerializer.Deserializer);
        }
    }
}
