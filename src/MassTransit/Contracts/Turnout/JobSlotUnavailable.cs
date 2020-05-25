namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobSlotUnavailable
    {
        Guid JobId { get; }
    }
}
