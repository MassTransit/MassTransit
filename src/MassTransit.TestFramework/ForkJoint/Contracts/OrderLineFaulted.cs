namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface OrderLineFaulted :
        FutureFaulted
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        string Description { get; }
    }
}
