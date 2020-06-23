namespace UsageContracts
{
    using System;

    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderSubmitted
    {
        Guid OrderId { get; }
    }

    public interface SubmitOrderAcknowledged
    {
        Guid OrderId { get; }
    }
}
