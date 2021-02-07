namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;
    using Topology;


    [ExcludeFromTopology]
    public interface OrderLine
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
    }
}