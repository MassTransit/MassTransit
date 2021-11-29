namespace MassTransit.Abstractions.Tests.Usage
{
    using System;


    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }
}
