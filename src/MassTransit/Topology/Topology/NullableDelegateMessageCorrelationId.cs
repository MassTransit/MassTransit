namespace MassTransit.Topology
{
    using System;


    public class NullableDelegateMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class
    {
        readonly Func<T, Guid?> _getCorrelationId;

        public NullableDelegateMessageCorrelationId(Func<T, Guid?> getCorrelationId)
        {
            _getCorrelationId = getCorrelationId;
        }

        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            Guid? id = _getCorrelationId(message);
            if (id.HasValue && id.Value != Guid.Empty)
            {
                correlationId = id.Value;
                return true;
            }

            correlationId = Guid.Empty;
            return false;
        }
    }
}
