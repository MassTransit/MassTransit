namespace MassTransit.Scoping
{
    using System;
    using Courier;


    public interface IExecuteActivityScopeContext<out TActivity, TArguments> :
        IDisposable
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        ExecuteActivityContext<TActivity, TArguments> Context { get; }
    }
}