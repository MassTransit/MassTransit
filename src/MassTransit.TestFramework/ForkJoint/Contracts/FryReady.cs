namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface FryReady
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        Size Size { get; }
    }
}
