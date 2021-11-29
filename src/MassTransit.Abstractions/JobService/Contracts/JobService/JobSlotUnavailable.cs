namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobSlotUnavailable
    {
        Guid JobId { get; }
    }
}
