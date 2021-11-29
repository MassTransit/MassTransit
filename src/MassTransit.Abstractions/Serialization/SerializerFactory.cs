namespace MassTransit
{
    using System.Net.Mime;


    public delegate IMessageSerializer SerializerFactory();


    public interface ISerializerFactory
    {
        ContentType ContentType { get; }

        IMessageSerializer CreateSerializer();

        IMessageDeserializer CreateDeserializer();
    }
}
