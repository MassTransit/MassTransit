namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface JobSubmissionAccepted
    {
        Guid JobId { get; }
    }
}
