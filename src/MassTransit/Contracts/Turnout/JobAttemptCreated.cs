namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobAttemptCreated
    {
        Guid JobId { get; }

        Guid AttemptId { get; }

        int RetryAttempt { get; }
    }
}
