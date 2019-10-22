namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;


    /// <summary>
    /// A context for a message consumed within a message session
    /// </summary>
    public interface MessageSessionContext
    {
        /// <summary>
        /// The SessionId of the session
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// The session is locked until...
        /// </summary>
        DateTime LockedUntilUtc { get; }

        /// <summary>
        /// Returns the state as a stream
        /// </summary>
        /// <returns></returns>
        Task<byte[]> GetStateAsync();

        /// <summary>
        /// Writes the message state from the specified stream
        /// </summary>
        /// <param name="sessionState"></param>
        /// <returns></returns>
        Task SetStateAsync(byte[] sessionState);

        /// <summary>
        /// Renews the session lock
        /// </summary>
        /// <returns></returns>
        Task RenewLockAsync(Message message);
    }
}
