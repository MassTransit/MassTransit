namespace UsageMessageSetCorrelation
{
    using System;
    using UsageContracts;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Topology.Topologies;


    public class Program
    {
        public static void Main()
        {
            // Use the OrderId as the message CorrelationId
            GlobalTopology.Send.UseCorrelationId<SubmitOrder>(x => x.OrderId);

            // Previous approach, which now calls the new way above
            MessageCorrelation.UseCorrelationId<SubmitOrder>(x => x.OrderId);
        }
    }
}