namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;


    public class AesCryptoStreamProvider :
        ICryptoStreamProvider
    {
        readonly string _defaultKeyId;
        readonly ISymmetricKeyProvider _keyProvider;
        readonly PaddingMode _paddingMode;

        public AesCryptoStreamProvider(ISymmetricKeyProvider keyProvider, string defaultKeyId, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _paddingMode = paddingMode;
            _keyProvider = keyProvider;
            _defaultKeyId = defaultKeyId;
        }

        Stream ICryptoStreamProvider.GetEncryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            keyId ??= _defaultKeyId;

            if (!_keyProvider.TryGetKey(keyId, out var key))
                throw new SerializationException("Encryption Key not found: " + keyId);

            var encryptor = CreateEncryptor(key.Key, key.IV);

            return new DisposingCryptoStream(stream, encryptor, streamMode);
        }

        Stream ICryptoStreamProvider.GetDecryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            keyId ??= _defaultKeyId;

            if (!_keyProvider.TryGetKey(keyId, out var key))
                throw new SerializationException("Encryption Key not found: " + keyId);

            var encryptor = CreateDecryptor(key.Key, key.IV);

            return new DisposingCryptoStream(stream, encryptor, streamMode);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("defaultKeyId", _defaultKeyId);
            context.Add("paddingMode", _paddingMode.ToString());
        }

        ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateDecryptor(key, iv);
            }
        }

        public ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
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
