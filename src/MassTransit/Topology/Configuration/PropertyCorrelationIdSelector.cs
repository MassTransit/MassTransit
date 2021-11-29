namespace MassTransit.Configuration
{
    using System;
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
            var propertyInfo = typeof(T).GetProperty(_propertyName);
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                messageCorrelationId = new PropertyMessageCorrelationId<T>(propertyInfo);
                return true;
            }

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid?))
            {
                messageCorrelationId = new NullablePropertyMessageCorrelationId<T>(propertyInfo);
                return true;
            }

            messageCorrelationId = null;
            return false;
        }
    }
}
