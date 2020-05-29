namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using System.Reflection;
    using Internals.Reflection;


    public class NullablePropertySetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        readonly IReadProperty<T, Guid?> _property;

        public NullablePropertySetCorrelationId(PropertyInfo propertyInfo)
        {
            _property = ReadPropertyCache<T>.GetProperty<Guid?>(propertyInfo);
        }

        void ISetCorrelationId<T>.SetCorrelationId(SendContext<T> context)
        {
            Guid? correlationId = _property.Get(context.Message);
            if (correlationId.HasValue && correlationId.Value != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}
