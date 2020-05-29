namespace MassTransit
{
    using System.Net.Mime;
    using GreenPipes;


    public interface IMessageDeserializer :
        IProbeSite
    {
        ContentType ContentType { get; }

        ConsumeContext Deserialize(ReceiveContext receiveContext);
    }
}
