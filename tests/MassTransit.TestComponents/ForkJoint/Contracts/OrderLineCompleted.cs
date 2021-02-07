namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;


    public interface OrderLineCompleted :
        FutureCompleted
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        string Description { get; }
    }
}