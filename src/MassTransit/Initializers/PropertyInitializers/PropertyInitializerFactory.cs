namespace MassTransit.Initializers.PropertyInitializers
{
    public class PropertyInitializerFactory<TProperty, TInputProperty> :
        IPropertyInitializerFactory<TProperty>
    {
        readonly IPropertyConverter<TProperty, TInputProperty> _converter;

        public PropertyInitializerFactory(IPropertyConverter<TProperty, TInputProperty> converter)
        {
            _converter = converter;
        }

        public IPropertyInitializer<TMessage, TInput> CreatePropertyInitializer<TMessage, TInput>(string messagePropertyName,
            string inputPropertyName)
            where TMessage : class
            where TInput : class
        {
            return new ConvertObjectPropertyInitializer<TMessage, TInput, TProperty, TInputProperty>(_converter, messagePropertyName,
                inputPropertyName);
        }

        public bool IsPropertyTypeConverter<T>(out ITypeConverter<TProperty, T> typeConverter)
        {
            typeConverter = default;
            return false;
        }

        public bool IsMessagePropertyConverter<T>(out IPropertyConverter<TProperty, T> propertyConverter)
        {
            propertyConverter = _converter as IPropertyConverter<TProperty, T>;

            return propertyConverter != null;
        }
    }
}
