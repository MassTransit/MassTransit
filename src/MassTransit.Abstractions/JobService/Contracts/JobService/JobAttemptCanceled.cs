namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobAttemptCanceled
    {
        Guid JobId { get; }
        Guid AttemptId { get; }
        DateTime Timestamp { get; }
        string Reason { get; }
    }
}
