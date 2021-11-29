namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


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
        Task<BinaryData> GetStateAsync();

        /// <summary>
        /// Writes the message state from the specified stream
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task SetStateAsync(BinaryData state);

        /// <summary>
        /// Renews the session lock
        /// </summary>
        /// <returns></returns>
        Task RenewLockAsync(ServiceBusReceivedMessage message);
    }
}
