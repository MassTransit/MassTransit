namespace MassTransit.Internals.Mapping
{
    using System.ComponentModel;
    using Reflection;


    public class ValueObjectMapper<T> :
        IObjectMapper<T>
    {
        readonly ReadWriteProperty<T> _property;
        readonly TypeConverter _typeConverter;

        public ValueObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
            _typeConverter = TypeDescriptor.GetConverter(property.Property.PropertyType);
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            object value;
            if (valueProvider.TryGetValue(_property.Property.Name, out value))
            {
                if (value != null)
                {
                    var valueType = value.GetType();
                    if (!valueType.IsInstanceOfType(_property.Property.PropertyType))
                    {
                        if (_typeConverter.IsValid(value))
                        {
                            if (_typeConverter.CanConvertFrom(valueType))
                                value = _typeConverter.ConvertFrom(value);
                        }
                    }
                }
                
                if (value != null)
                    _property.Set(obj, value);
            }
        }
    }
}