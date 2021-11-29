namespace MassTransit.Abstractions.Tests.Usage
{
    using System;


    [ExcludeFromTopology]
    public interface OrderEvent :
        CorrelatedBy<Guid>
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}
