namespace MassTransit.Initializers.Conventions
{
    public interface IInitializerConvention<TMessage, TInput> :
        IMessageTypeInitializerConvention<TMessage>
        where TMessage : class
        where TInput : class
    {
        bool TryGetMessagePropertyInitializer<TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer);
    }


    public interface IInitializerConvention<TMessage> :
        IMessageTypeInitializerConvention
        where TMessage : class
    {
        bool TryGetMessagePropertyInitializer<TInput, TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class;
    }


    public interface IInitializerConvention
    {
        bool TryGetMessagePropertyInitializer<TMessage, TInput, TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class;
    }
}
