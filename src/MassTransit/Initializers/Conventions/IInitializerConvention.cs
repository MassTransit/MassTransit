namespace MassTransit.Initializers.Conventions
{
    using System.Reflection;


    public interface IInitializerConvention<TMessage, TInput> :
        IMessageInputInitializerConvention<TMessage>
        where TMessage : class
        where TInput : class
    {
        bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer);
        bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer);
        bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer);
    }


    public interface IInitializerConvention<TMessage> :
        IMessageInitializerConvention
        where TMessage : class
    {
        bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class;

        bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class;

        bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class;
    }


    public interface IInitializerConvention
    {
        bool TryGetPropertyInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;

        bool TryGetHeaderInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;

        bool TryGetHeadersInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;
    }
}
