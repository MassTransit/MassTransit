namespace MassTransit.DependencyInjection
{
    using System;


    public interface IExecuteScopeContext<out TArguments> :
        IAsyncDisposable
        where TArguments : class
    {
        ExecuteContext<TArguments> Context { get; }

        T GetService<T>()
            where T : class;
    }
}
