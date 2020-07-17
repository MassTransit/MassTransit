namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobAttemptCanceled
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }
        DateTime Timestamp { get; }
    }
}
