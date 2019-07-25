namespace MassTransit.Initializers.PropertyInitializers
{
    public interface IPropertyInitializerFactory<TProperty> :
        IPropertyInitializerFactory
    {
        bool IsPropertyTypeConverter<T>(out ITypeConverter<TProperty, T> typeConverter);

        bool IsMessagePropertyConverter<T>(out IPropertyConverter<TProperty, T> propertyConverter);
    }


    public interface IPropertyInitializerFactory
    {
        IPropertyInitializer<TMessage, TInput> CreatePropertyInitializer<TMessage, TInput>(string messagePropertyName,
            IPropertyProviderFactory<TInput> providerFactory)
            where TInput : class
            where TMessage : class;

        IHeaderInitializer<TMessage, TInput> CreateHeaderInitializer<TMessage, TInput>(string headerPropertyName,
            IPropertyProviderFactory<TInput> providerFactory)
            where TInput : class
            where TMessage : class;
    }
}
