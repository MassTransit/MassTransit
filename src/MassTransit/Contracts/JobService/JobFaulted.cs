namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a job faults
    /// </summary>
    public interface JobFaulted
    {
        Guid JobId { get; }

        DateTime Timestamp { get; }

        TimeSpan? Duration { get; }

        IDictionary<string, object> Job { get; }

        ExceptionInfo Exceptions { get; }
    }
}
