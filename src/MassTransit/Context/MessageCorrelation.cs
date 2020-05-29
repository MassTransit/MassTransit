namespace MassTransit.Context
{
    using System;
    using Topology.Topologies;


    public static class MessageCorrelation
    {
        public static void UseCorrelationId<T>(Func<T, Guid> getCorrelationId)
            where T : class
        {
            GlobalTopology.Send.UseCorrelationId(getCorrelationId);
        }
    }
}
