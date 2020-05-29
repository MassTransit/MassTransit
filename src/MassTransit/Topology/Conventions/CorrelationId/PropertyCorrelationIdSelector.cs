namespace MassTransit.Topology.Conventions.CorrelationId
{
    using System;
    using Context;
    using Context.SetCorrelationIds;


    public class PropertyCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        readonly string _propertyName;

        public PropertyCorrelationIdSelector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool TryGetSetCorrelationId(out ISetCorrelationId<T> setCorrelationId)
        {
            var propertyInfo = typeof(T).GetProperty(_propertyName);
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                setCorrelationId = new PropertySetCorrelationId<T>(propertyInfo);
                return true;
            }

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid?))
            {
                setCorrelationId = new NullablePropertySetCorrelationId<T>(propertyInfo);
                return true;
            }

            setCorrelationId = null;
            return false;
        }
    }
}
