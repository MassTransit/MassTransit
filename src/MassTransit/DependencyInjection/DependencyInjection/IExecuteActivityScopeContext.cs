namespace MassTransit.DependencyInjection
{
    using System;


    public interface IExecuteActivityScopeContext<out TActivity, out TArguments> :
        IAsyncDisposable
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ExecuteActivityContext<TActivity, TArguments> Context { get; }

        T GetService<T>()
            where T : class;
    }
}
