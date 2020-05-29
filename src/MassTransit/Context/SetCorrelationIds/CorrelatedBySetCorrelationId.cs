namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using Internals.Reflection;


    public class CorrelatedBySetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class, CorrelatedBy<Guid>
    {
        readonly IReadProperty<CorrelatedBy<Guid>, Guid> _property;

        public CorrelatedBySetCorrelationId()
        {
            _property = ReadPropertyCache<CorrelatedBy<Guid>>.GetProperty<Guid>(nameof(CorrelatedBy<Guid>.CorrelationId));
        }

        public void SetCorrelationId(SendContext<T> context)
        {
            var message = (CorrelatedBy<Guid>)context.Message;

            var correlationId = _property.Get(message);
            if (correlationId != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}
