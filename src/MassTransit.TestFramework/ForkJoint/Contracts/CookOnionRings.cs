namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface CookOnionRings
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }

        int Quantity { get; }
    }
}
