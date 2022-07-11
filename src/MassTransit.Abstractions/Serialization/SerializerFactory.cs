namespace MassTransit
{
    using System.Net.Mime;


    public interface ISerializerFactory
    {
        ContentType ContentType { get; }

        IMessageSerializer CreateSerializer();

        IMessageDeserializer CreateDeserializer();
    }
}
