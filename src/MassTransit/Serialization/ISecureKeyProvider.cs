namespace MassTransit.Serialization
{
    using GreenPipes;


    public interface ISecureKeyProvider : IProbeSite
    {
        byte[] GetKey(ReceiveContext receiveContext);

        byte[] GetKey(SendContext sendContext);
    }
}
