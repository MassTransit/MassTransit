namespace MassTransit
{
    using System;


    public interface IJobSagaOptionsConfigurator
    {
        /// <summary>
        /// The time to wait before attempting to allocate a job slot when no slots are available
        /// </summary>
        TimeSpan SlotWaitTime { set; }

        /// <summary>
        /// Time to wait before checking the status of a job to ensure it is still running (not dead)
        /// </summary>
        TimeSpan StatusCheckInterval { set; }

        /// <summary>
        /// Timeout on request to allocate a job slot
        /// </summary>
        TimeSpan SlotRequestTimeout { set; }

        /// <summary>
        /// Timeout to wait for a job to start
        /// </summary>
        TimeSpan StartJobTimeout { set; }

        /// <summary>
        /// The number of times to retry a suspect job before it is faulted. Defaults to zero.
        /// </summary>
        int SuspectJobRetryCount { set; }

        /// <summary>
        /// The delay before retrying a suspect job
        /// </summary>
        TimeSpan SuspectJobRetryDelay { set; }

        /// <summary>
        /// If specified, overrides the default saga partition count to reduce conflicts when using optimistic concurrency.
        /// If using a saga repository with pessimistic concurrency, this is not recommended.
        /// </summary>
        int? SagaPartitionCount { set; }

        /// <summary>
        /// If true, completed jobs are finalized, removing them from the saga repository
        /// </summary>
        bool FinalizeCompleted { set; }
    }
}
