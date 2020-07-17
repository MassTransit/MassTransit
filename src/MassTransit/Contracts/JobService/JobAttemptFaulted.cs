namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobAttemptFaulted
    {
        Guid JobId { get; }
        Guid AttemptId { get; }

        /// <summary>
        /// The retry attempt that faulted. Zero for the first attempt.
        /// </summary>
        int RetryAttempt { get; }

        /// <summary>
        /// If present, the delay until the next retry
        /// </summary>
        TimeSpan? RetryDelay { get; }

        DateTime Timestamp { get; }

        ExceptionInfo Exceptions { get; }
    }
}
