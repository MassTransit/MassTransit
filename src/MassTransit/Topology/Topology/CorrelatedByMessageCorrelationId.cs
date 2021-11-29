namespace MassTransit.Topology
{
    using System;


    public class CorrelatedByMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class, CorrelatedBy<Guid>
    {
        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            correlationId = message.CorrelationId;

            return correlationId != Guid.Empty;
        }
    }
}
