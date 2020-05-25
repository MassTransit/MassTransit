namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobSlotAllocated
    {
        Guid JobId { get; }
    }
}
