namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using System.Reflection;
    using Internals.Reflection;


    public class PropertySetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        readonly IReadProperty<T, Guid> _property;

        public PropertySetCorrelationId(PropertyInfo propertyInfo)
        {
            _property = ReadPropertyCache<T>.GetProperty<Guid>(propertyInfo);
        }

        void ISetCorrelationId<T>.SetCorrelationId(SendContext<T> context)
        {
            var correlationId = _property.Get(context.Message);
            if (correlationId != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}
