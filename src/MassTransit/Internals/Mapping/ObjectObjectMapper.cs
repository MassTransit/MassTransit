namespace MassTransit.Internals.Mapping
{
    using System.ComponentModel;
    using Reflection;


    public class ObjectObjectMapper<T> :
        IObjectMapper<T>
    {
        readonly IObjectConverter _converter;
        readonly ReadWriteProperty<T> _property;
        readonly TypeConverter _typeConverter;

        public ObjectObjectMapper(ReadWriteProperty<T> property,
            IObjectConverter converter)
        {
            _property = property;
            _converter = converter;
            _typeConverter = TypeDescriptor.GetConverter(property.Property.PropertyType);
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            IObjectValueProvider propertyProvider;
            if (valueProvider.TryGetValue(_property.Property.Name, out propertyProvider))
            {
                object value = _converter.GetObject(propertyProvider);
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

                _property.Set(obj, value);
            }
        }
    }
}