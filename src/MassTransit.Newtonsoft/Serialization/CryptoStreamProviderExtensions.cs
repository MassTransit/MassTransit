namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Security.Cryptography;


    public static class CryptoStreamProviderExtensions
    {
        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetEncryptStream(this ICryptoStreamProvider provider, Stream stream, SendContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out var keyIdObj)
                ? keyIdObj.ToString()
                : default;

            return provider.GetEncryptStream(stream, keyId, CryptoStreamMode.Write);
        }

        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetEncryptStream(this ICryptoStreamProvider provider, Stream stream, TransformContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out var keyIdObj)
                ? keyIdObj.ToString()
                : default;

            return provider.GetEncryptStream(stream, keyId, CryptoStreamMode.Write);
        }

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="headers"></param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetDecryptStream(this ICryptoStreamProvider provider, Stream stream, Headers headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            var keyId = headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out var keyIdObj)
                ? keyIdObj.ToString()
                : default;

            return provider.GetDecryptStream(stream, keyId, CryptoStreamMode.Read);
        }

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetDecryptStream(this ICryptoStreamProvider provider, Stream stream, TransformContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out var keyIdObj)
                ? keyIdObj.ToString()
                : default;

            return provider.GetDecryptStream(stream, keyId, CryptoStreamMode.Read);
        }

        /// <summary>
        /// Set the encryption key name on the header so that it can be applied by the crypto stream provider
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="keyName"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetEncryptionKeyId<T>(this IPipeConfigurator<SendContext<T>> configurator, string keyName)
            where T : class
        {
            configurator.UseExecute(context => context.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, keyName));
        }

        /// <summary>
        /// Set the encryption key name on the header so that it can be applied by the crypto stream provider
        /// </summary>
        /// <param name="sendContext"></param>
        /// <param name="keyName"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetEncryptionKeyId<T>(this SendContext<T> sendContext, string keyName)
            where T : class
        {
            sendContext.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, keyName);
        }
    }
}
