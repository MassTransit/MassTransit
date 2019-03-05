namespace MassTransit.Initializers.PropertyInitializers
{
    using HeaderInitializers;
    using PropertyConverters;
    using PropertyProviders;


    public class TypeConverterPropertyInitializerFactory<TProperty, TInputProperty> :
        IPropertyInitializerFactory<TProperty>
    {
        readonly ITypeConverter<TProperty, TInputProperty> _converter;

        public TypeConverterPropertyInitializerFactory(ITypeConverter<TProperty, TInputProperty> converter)
        {
            _converter = converter;
        }

        public IPropertyInitializer<TMessage, TInput> CreatePropertyInitializer<TMessage, TInput>(string messagePropertyName,
            string inputPropertyName)
            where TMessage : class
            where TInput : class
        {
            var provider = new TypeConvertInputValuePropertyProvider<TInput, TProperty, TInputProperty>(_converter, inputPropertyName ?? messagePropertyName);

            return new ProviderPropertyInitializer<TMessage, TInput, TProperty>(provider, messagePropertyName);
        }

        public IHeaderInitializer<TMessage, TInput> CreateHeaderInitializer<TMessage, TInput>(string headerPropertyName, string inputPropertyName = null)
            where TMessage : class
            where TInput : class
        {
            var provider = new TypeConvertInputValuePropertyProvider<TInput, TProperty, TInputProperty>(_converter, inputPropertyName ?? headerPropertyName);

            return new ProviderHeaderInitializer<TMessage, TInput, TProperty>(provider, headerPropertyName);
        }

        public bool IsPropertyTypeConverter<T>(out ITypeConverter<TProperty, T> typeConverter)
        {
            typeConverter = _converter as ITypeConverter<TProperty, T>;

            return typeConverter != null;
        }

        public bool IsMessagePropertyConverter<T>(out IPropertyConverter<TProperty, T> propertyConverter)
        {
            if (_converter is ITypeConverter<TProperty, T> converter)
            {
                propertyConverter = new ConvertPropertyConverter<TProperty, T>(converter);
                return true;
            }

            propertyConverter = null;
            return false;
        }
    }
}
