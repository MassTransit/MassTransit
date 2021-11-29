namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface OnionRingsReady
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        int Quantity { get; }
    }
}
