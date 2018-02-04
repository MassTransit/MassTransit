namespace MassTransit.Initializers
{
    public interface IMessageInitializerFactory<TMessage>
        where TMessage : class
    {
        IMessageInitializer<TMessage> CreateMessageInitializer();
    }
}