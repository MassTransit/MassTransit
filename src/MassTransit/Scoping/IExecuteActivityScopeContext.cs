namespace MassTransit.Scoping
{
    using System;
    using Courier;


    public interface IExecuteActivityScopeContext<out TActivity, out TArguments> :
        IDisposable
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ExecuteActivityContext<TActivity, TArguments> Context { get; }
    }
}
