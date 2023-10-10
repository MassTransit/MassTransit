namespace MassTransit.Configuration
{
    using System;
    using Internals;
    using Topology;


    public class PropertyCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        readonly string _propertyName;

        public PropertyCorrelationIdSelector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
        {
            if (ReadPropertyCache<T>.TryGetProperty(_propertyName, out IReadProperty<T, Guid> property))
            {
                messageCorrelationId = new PropertyMessageCorrelationId<T>(property);
                return true;
            }

            if (ReadPropertyCache<T>.TryGetProperty(_propertyName, out IReadProperty<T, Guid?> nullableProperty))
            {
                messageCorrelationId = new NullablePropertyMessageCorrelationId<T>(nullableProperty);
                return true;
            }

            messageCorrelationId = null;
            return false;
        }
    }
}
