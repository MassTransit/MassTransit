namespace MassTransit
{
    using System;


    public static class MessageCorrelation
    {
        public static void UseCorrelationId<T>(Func<T, Guid> getCorrelationId)
            where T : class
        {
            GlobalTopology.Send.UseCorrelationId(getCorrelationId);
        }
    }
}
