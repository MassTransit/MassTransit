using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface IOnRampSemaphore
    {
        Task<bool> ObtainLock(
            Guid requestorId,
            ILockRepository? repository,
            string lockName,
            CancellationToken cancellationToken = default);

        /// <summary> Release the lock on the identified resource if it is held by the calling
        /// thread.
        /// </summary>
        Task ReleaseLock(
            Guid requestorId,
            string lockName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Whether this Semaphore implementation requires a database connection for
        /// its lock management operations.
        /// </summary>
        /// <seealso cref="ObtainLock" />
        /// <seealso cref="ReleaseLock" />
        bool RequiresConnection { get; }
    }
}
