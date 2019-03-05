namespace MassTransit.Initializers.PropertyInitializers
{
    using HeaderInitializers;
    using PropertyProviders;


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

        public IHeaderInitializer<TMessage, TInput> CreateHeaderInitializer<TMessage, TInput>(string headerPropertyName, string inputPropertyName = null)
            where TMessage : class
            where TInput : class
        {
            var provider = new PropertyConvertInputValuePropertyProvider<TInput, TProperty, TInputProperty>(_converter, inputPropertyName ??
                headerPropertyName);

            return new ProviderHeaderInitializer<TMessage, TInput, TProperty>(provider, headerPropertyName);
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
