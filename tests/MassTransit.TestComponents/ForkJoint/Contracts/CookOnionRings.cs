namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;


    public interface CookOnionRings
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }

        int Quantity { get; }
    }
}