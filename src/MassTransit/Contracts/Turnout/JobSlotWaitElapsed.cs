namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobSlotWaitElapsed
    {
        Guid JobId { get; }
    }
}
