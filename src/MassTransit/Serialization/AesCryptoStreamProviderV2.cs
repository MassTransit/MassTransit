namespace MassTransit.Serialization
{
    using System.IO;
    using System.Security.Cryptography;
    using GreenPipes;


    public class AesCryptoStreamProviderV2 : ICryptoStreamProviderV2
    {
        readonly PaddingMode _paddingMode;
        readonly ISecureKeyProvider _secureKeyProvider;

        public AesCryptoStreamProviderV2(ISecureKeyProvider secureKeyProvider,
            PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _secureKeyProvider = secureKeyProvider;
            _paddingMode = paddingMode;
        }

        public Stream GetDecryptStream(Stream stream, ReceiveContext context)
        {
            byte[] key = _secureKeyProvider.GetKey(context);

            var iv = new byte[16];
            stream.Read(iv, 0, iv.Length);

            var encryptor = CreateDecryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Read);
        }

        public Stream GetEncryptStream(Stream stream, SendContext context)
        {
            byte[] key = _secureKeyProvider.GetKey(context);

            byte[] iv = GenerateIv();

            stream.Write(iv, 0, iv.Length);
            var encryptor = CreateEncryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Write);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("aes");

            scope.Add("paddingMode", _paddingMode.ToString());

            _secureKeyProvider.Probe(scope);
        }

        ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateDecryptor(key, iv);
            }
        }

        byte[] GenerateIv()
        {
            using (var aes = CreateAes())
            {
                aes.GenerateIV();

                return aes.IV;
            }
        }

        ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateEncryptor(key, iv);
            }
        }

        Aes CreateAes()
        {
            return new AesCryptoServiceProvider {Padding = _paddingMode};
        }
    }
}
