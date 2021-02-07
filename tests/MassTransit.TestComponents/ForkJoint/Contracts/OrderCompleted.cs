namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;
    using System.Collections.Generic;


    public interface OrderCompleted :
        FutureCompleted
    {
        Guid OrderId { get; }

        IDictionary<Guid, OrderLineCompleted> LinesCompleted { get; }
    }
}