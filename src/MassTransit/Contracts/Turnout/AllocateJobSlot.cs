namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface AllocateJobSlot
    {
        Guid JobTypeId { get; }

        TimeSpan JobTimeout { get; }

        Guid JobId { get; }
    }
}
