namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    public interface JobAttemptCanceled
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }
        DateTime Timestamp { get; }
        IDictionary<string, object> Job { get; }
    }
}
