using MassTransit.Context;
using MassTransit.Transports.Outbox.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class DbSemaphore : IOutboxSemaphore
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<Guid, HashSet<string>> locks = new Dictionary<Guid, HashSet<string>>();
        private readonly IOutboxTransportOptions _outboxTransportOptions;

        /// <summary>
        /// This Semaphore implementation does use the database.
        /// </summary>
        public bool RequiresConnection => true;

        public DbSemaphore(IOutboxTransportOptions outboxTransportOptions)
        {
            _outboxTransportOptions = outboxTransportOptions;
        }

        public async Task<bool> ObtainLock(Guid requestorId, ILockRepository locksRepository, string lockName, CancellationToken cancellationToken = default)
        {
            LogContext.Debug?.Log("Lock '{0}' is desired by: {1}", lockName, requestorId);
            if (!IsLockOwner(requestorId, lockName))
            {
                await locksRepository.ObtainLock(_outboxTransportOptions.OutboxName, lockName);

                LogContext.Debug?.Log("Lock '{0}' given to: {1}", lockName, requestorId);

                lock (syncRoot)
                {
                    if (!locks.TryGetValue(requestorId, out var requestorLocks))
                    {
                        requestorLocks = new HashSet<string>();
                        locks[requestorId] = requestorLocks;
                    }
                    requestorLocks.Add(lockName);
                }
            }
            else if (LogContext.Debug != null)
            {
                LogContext.Debug?.Log("Lock '{0}' Is already owned by: {1}", lockName, requestorId);
            }

            return true;
        }

        public Task ReleaseLock(Guid requestorId, string lockName, CancellationToken cancellationToken = default)
        {
            if (IsLockOwner(requestorId, lockName))
            {
                lock (syncRoot)
                {
                    if (locks.TryGetValue(requestorId, out var requestorLocks))
                    {
                        requestorLocks.Remove(lockName);
                        if (requestorLocks.Count == 0)
                        {
                            locks.Remove(requestorId);
                        }
                    }
                }
                LogContext.Debug?.Log("Lock '{0}' returned by: {1}", lockName, requestorId);
            }
            else if (LogContext.Warning != null)
            {
                LogContext.Warning?.Log($"Lock '{lockName}' attempt to return by: {requestorId} -- but not owner!",
                    new Exception("stack-trace of wrongful returner"));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Determine whether the calling thread owns a lock on the identified
        /// resource.
        /// </summary>
        private bool IsLockOwner(Guid requestorId, string lockName)
        {
            lock (syncRoot)
            {
                return locks.TryGetValue(requestorId, out var requestorLocks) && requestorLocks.Contains(lockName);
            }
        }
    }
}
