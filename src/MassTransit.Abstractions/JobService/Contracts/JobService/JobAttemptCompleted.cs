namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    public interface JobAttemptCompleted
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }
        DateTime Timestamp { get; }
        TimeSpan Duration { get; }
        Dictionary<string, object>? InstanceProperties { get; }
        Dictionary<string, object>? JobTypeProperties { get; }
    }
}
