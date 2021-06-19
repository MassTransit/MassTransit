namespace MassTransit.Contracts.JobService
{
    using System;


    public interface JobSlotReleased
    {
        Guid JobTypeId { get; }

        Guid JobId { get; }

        JobSlotDisposition Disposition { get; }
    }
}
