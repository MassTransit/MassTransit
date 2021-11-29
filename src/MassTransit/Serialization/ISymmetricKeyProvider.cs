namespace MassTransit.Serialization
{
    /// <summary>
    /// Returns the symmetric key used to encrypt or decrypt messages
    /// </summary>
    public interface ISymmetricKeyProvider
    {
        /// <summary>
        /// Return the specified key, if found. When using Symmetric key encryption, the default key is used
        /// unless the transport header contains a specific key identifier for the message.
        /// </summary>
        /// <param name="id">The key id</param>
        /// <param name="key">The symmetric key</param>
        /// <returns></returns>
        bool TryGetKey(string id, out SymmetricKey key);
    }
}
