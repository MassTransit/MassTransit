namespace MassTransit.Serialization
{
    using System.IO;
    using System.Security.Cryptography;


    /// <summary>
    /// Provides a crypto stream for the purpose of encrypting or decrypting
    /// </summary>
    public interface ICryptoStreamProvider :
        IProbeSite
    {
        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="keyId">The encryption key identifier</param>
        /// <param name="streamMode"></param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        Stream GetEncryptStream(Stream stream, string keyId, CryptoStreamMode streamMode);

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="stream">The input stream of the encrypted message</param>
        /// <param name="keyId">The encryption key identifier</param>
        /// <param name="streamMode"></param>
        /// <returns>A stream for deserializing the encrypted message</returns>
        Stream GetDecryptStream(Stream stream, string keyId, CryptoStreamMode streamMode);
    }
}
