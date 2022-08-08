namespace MassTransit.DependencyInjection
{
    using System;


    public interface IConsumeScopeContext :
        IAsyncDisposable
    {
        ConsumeContext Context { get; }
    }


    public interface IConsumeScopeContext<out TMessage> :
        IAsyncDisposable
        where TMessage : class
    {
        ConsumeContext<TMessage> Context { get; }

        T GetService<T>()
            where T : class;

        T CreateInstance<T>(params object[] arguments)
            where T : class;

        IDisposable PushConsumeContext(ConsumeContext context);
    }
}
