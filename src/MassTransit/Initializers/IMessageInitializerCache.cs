namespace MassTransit.Initializers
{
    using System;


    public interface IMessageInitializerCache<TMessage>
        where TMessage : class
    {
        IMessageInitializer<TMessage> GetInitializer(Type objectType);
    }
}
