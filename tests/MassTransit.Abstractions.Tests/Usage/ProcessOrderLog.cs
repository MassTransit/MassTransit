namespace MassTransit.Abstractions.Tests.Usage
{
    using System;


    public interface ProcessOrderLog
    {
        Guid OrderId { get; }
        Guid ShipmentId { get; }
    }
}
