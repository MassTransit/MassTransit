namespace MassTransit.Internals.Mapping
{
    using System;
    using Reflection;


    public class EnumObjectMapper<T> :
        IObjectMapper<T>
    {
        readonly ReadWriteProperty<T> _property;

        public EnumObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            object value;
            if (!valueProvider.TryGetValue(_property.Property.Name, out value))
                return;
            if (value == null)
                return;

            if (value is T)
            {
                _property.Set(obj, value);
                return;
            }

            var s = value as string;
            if (s != null)
            {
                object enumValue = Enum.Parse(_property.Property.PropertyType, s);
                _property.Set(obj, enumValue);
                return;
            }

            object n = Enum.ToObject(_property.Property.PropertyType, value);
            _property.Set(obj, n);
        }
    }
}