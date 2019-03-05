namespace MassTransit.Initializers.Factories
{
    public interface IMessageInitializerBuilder<out TMessage, out TInput>
        where TMessage : class
        where TInput : class
    {
        void Add(string propertyName, IPropertyInitializer<TMessage> initializer);

        void Add(string propertyName, IPropertyInitializer<TMessage, TInput> initializer);

        void Add(IHeaderInitializer<TMessage> initializer);

        void Add(IHeaderInitializer<TMessage, TInput> initializer);

        bool IsInputPropertyUsed(string propertyName);

        void SetInputPropertyUsed(string propertyName);
    }
}
