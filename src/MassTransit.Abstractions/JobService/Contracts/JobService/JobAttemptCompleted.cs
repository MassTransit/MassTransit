namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobAttemptCompleted
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }
        DateTime Timestamp { get; }
        TimeSpan Duration { get; }
    }
}
