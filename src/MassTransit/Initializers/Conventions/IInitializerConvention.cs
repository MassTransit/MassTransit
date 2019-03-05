namespace MassTransit.Initializers.Conventions
{
    public interface IInitializerConvention<TMessage, TInput> :
        IMessageTypeInitializerConvention<TMessage>
        where TMessage : class
        where TInput : class
    {
        bool TryGetPropertyInitializer<TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer);
        bool TryGetHeaderInitializer<TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer);
    }


    public interface IInitializerConvention<TMessage> :
        IMessageTypeInitializerConvention
        where TMessage : class
    {
        bool TryGetPropertyInitializer<TInput, TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class;

        bool TryGetHeaderInitializer<TInput, TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class;
    }


    public interface IInitializerConvention
    {
        bool TryGetPropertyInitializer<TMessage, TInput, TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;

        bool TryGetHeaderInitializer<TMessage, TInput, TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;
    }
}
