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
            string inputPropertyName = null)
            where TInput : class
            where TMessage : class;
    }
}
