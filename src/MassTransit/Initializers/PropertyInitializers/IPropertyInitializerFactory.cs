namespace MassTransit.Initializers.PropertyInitializers
{
    using System.Reflection;


    public interface IPropertyInitializerFactory<TProperty> :
        IPropertyInitializerFactory
    {
        bool IsPropertyTypeConverter<T>(out ITypeConverter<TProperty, T> typeConverter);

        bool IsMessagePropertyConverter<T>(out IPropertyConverter<TProperty, T> propertyConverter);
    }


    public interface IPropertyInitializerFactory
    {
        IPropertyInitializer<TMessage, TInput> CreatePropertyInitializer<TMessage, TInput>(PropertyInfo propertyInfo,
            IPropertyProviderFactory<TInput> providerFactory)
            where TInput : class
            where TMessage : class;

        IHeaderInitializer<TMessage, TInput> CreateHeaderInitializer<TMessage, TInput>(PropertyInfo propertyInfo,
            IPropertyProviderFactory<TInput> providerFactory)
            where TInput : class
            where TMessage : class;
    }
}
