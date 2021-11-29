namespace MassTransit.Abstractions.Tests.Usage
{
    using System;


    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; }
    }
}
