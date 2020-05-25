namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobSlotReleased
    {
        Guid JobTypeId { get; }

        Guid JobId { get; }
    }
}
