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
            IPropertyProviderFactory<TInput> providerFactory)
            where TMessage : class
            where TInput : class
        {
            var provider = CreatePropertyProvider(providerFactory);

            return new ProviderPropertyInitializer<TMessage, TInput, TProperty>(provider, messagePropertyName);
        }

        public IHeaderInitializer<TMessage, TInput> CreateHeaderInitializer<TMessage, TInput>(string headerPropertyName,
            IPropertyProviderFactory<TInput> providerFactory)
            where TMessage : class
            where TInput : class
        {
            var provider = CreatePropertyProvider(providerFactory);

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

        IPropertyProvider<TInput, TProperty> CreatePropertyProvider<TInput>(IPropertyProviderFactory<TInput> providerFactory)
            where TInput : class
        {
            var inputPropertyProvider = providerFactory.CreatePropertyProvider<TInputProperty>();

            return new PropertyConverterPropertyProvider<TInput, TProperty, TInputProperty>(_converter, inputPropertyProvider);
        }
    }
}
