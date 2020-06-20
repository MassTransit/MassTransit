namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using Internals.Reflection;


    public class CorrelatedByMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class, CorrelatedBy<Guid>
    {
        readonly IReadProperty<CorrelatedBy<Guid>, Guid> _property;

        public CorrelatedByMessageCorrelationId()
        {
            _property = ReadPropertyCache<CorrelatedBy<Guid>>.GetProperty<Guid>(nameof(CorrelatedBy<Guid>.CorrelationId));
        }

        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            correlationId = _property.Get(message);

            return correlationId != Guid.Empty;
        }
    }
}
