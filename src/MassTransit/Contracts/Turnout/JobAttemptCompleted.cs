namespace MassTransit.Contracts.Turnout
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
        IDictionary<string, object> Job { get; }
    }
}
