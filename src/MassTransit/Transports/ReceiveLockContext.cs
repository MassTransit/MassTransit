namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Encapsulates a transport lock
    /// </summary>
    public interface ReceiveLockContext
    {
        /// <summary>
        /// Called to complete the message
        /// </summary>
        /// <returns></returns>
        Task Complete();

        /// <summary>
        /// Called if the message was faulted. This method should NOT throw an exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task Faulted(Exception exception);

        /// <summary>
        /// Validate that the lock is still valid
        /// </summary>
        /// <returns></returns>
        Task ValidateLockStatus();
    }
}
