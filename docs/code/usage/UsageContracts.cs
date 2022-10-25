namespace UsageContracts;

using System;

public record SubmitOrder
{
    public Guid OrderId { get; init; }
}

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
}

public record SubmitOrderAcknowledged
{
    public Guid OrderId { get; init; }
}
