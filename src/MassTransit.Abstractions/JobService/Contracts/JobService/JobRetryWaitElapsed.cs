namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobRetryDelayElapsed
    {
        Guid JobId { get; }
    }
}
