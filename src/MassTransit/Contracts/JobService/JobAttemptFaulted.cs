namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    public interface JobAttemptFaulted
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        int RetryAttempt { get; }
        IDictionary<string, object> Job { get; }
        DateTime Timestamp { get; }
        ExceptionInfo Exceptions { get; }
    }
}
