namespace ContainerContracts
{
    using System;

    public record SubmitOrder
    {
        public Guid OrderId { get; init; }
    }

    public record OrderSubmitted
    {
        public Guid OrderId { get; init; }
    }
}
