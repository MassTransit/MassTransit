namespace MassTransit.Internals.Mapping
{
    using Reflection;


    public class NullableValueObjectMapper<T, TValue> :
        IObjectMapper<T>
        where TValue : struct
    {
        readonly ReadWriteProperty<T> _property;

        public NullableValueObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            object value;
            if (valueProvider.TryGetValue(_property.Property.Name, out value))
            {
                var nullableValue = (TValue?)value;
                _property.Set(obj, nullableValue);
            }
        }
    }
}