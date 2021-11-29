namespace MassTransit.Topology
{
    using System;
    using System.Reflection;
    using Internals;


    public class NullablePropertyMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class
    {
        readonly IReadProperty<T, Guid?> _property;

        public NullablePropertyMessageCorrelationId(PropertyInfo propertyInfo)
        {
            _property = ReadPropertyCache<T>.GetProperty<Guid?>(propertyInfo);
        }

        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            Guid? id = _property.Get(message);
            if (id.HasValue && id.Value != Guid.Empty)
            {
                correlationId = id.Value;
                return true;
            }

            return false;
        }
    }
}
