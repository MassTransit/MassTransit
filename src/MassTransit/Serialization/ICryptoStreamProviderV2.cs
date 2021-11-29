namespace MassTransit.Serialization
{
    using System.IO;


    public interface ICryptoStreamProviderV2 :
        IProbeSite
    {
        Stream GetDecryptStream(Stream stream, Headers headers);

        Stream GetEncryptStream(Stream stream, Headers headers);
    }
}
