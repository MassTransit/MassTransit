namespace MassTransit.Serialization
{
    using System.IO;
    using GreenPipes;


    public interface ICryptoStreamProviderV2 : IProbeSite
    {
        Stream GetDecryptStream(Stream stream, ReceiveContext context);

        Stream GetEncryptStream(Stream stream, SendContext context);
    }
}
