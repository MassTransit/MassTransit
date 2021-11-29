namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobSlotWaitElapsed
    {
        Guid JobId { get; }
    }
}
