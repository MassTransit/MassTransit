namespace MassTransit.Topology
{
    using System;
    using Context;


    public class DelegateMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class
    {
        readonly Func<T, Guid> _getCorrelationId;

        public DelegateMessageCorrelationId(Func<T, Guid> getCorrelationId)
        {
            _getCorrelationId = getCorrelationId;
        }

        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            correlationId = _getCorrelationId(message);

            return correlationId != Guid.Empty;
        }
    }
}
