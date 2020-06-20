namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using System.Reflection;
    using Internals.Reflection;


    public class PropertyMessageCorrelationId<T> :
        IMessageCorrelationId<T>
        where T : class
    {
        readonly IReadProperty<T, Guid> _property;

        public PropertyMessageCorrelationId(PropertyInfo propertyInfo)
        {
            _property = ReadPropertyCache<T>.GetProperty<Guid>(propertyInfo);
        }

        public bool TryGetCorrelationId(T message, out Guid correlationId)
        {
            correlationId = _property.Get(message);

            return correlationId != Guid.Empty;
        }
    }
}
