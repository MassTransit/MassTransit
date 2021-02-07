namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;
    using System.Collections.Generic;
    using MassTransit;


    public interface OrderFaulted :
        FutureFaulted
    {
        Guid OrderId { get; }

        IDictionary<Guid, OrderLineCompleted> LinesCompleted { get; }

        IDictionary<Guid, Fault<OrderLine>> LinesFaulted { get; }
    }
}