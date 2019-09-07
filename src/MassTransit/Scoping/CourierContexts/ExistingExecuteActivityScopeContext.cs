namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using Courier;


    public class ExistingExecuteActivityScopeContext<TActivity, TArguments> :
        IExecuteActivityScopeContext<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Action<TActivity> _disposeCallback;

        public ExistingExecuteActivityScopeContext(ExecuteActivityContext<TActivity, TArguments> context, Action<TActivity> disposeCallback = null)
        {
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public void Dispose()
        {
            _disposeCallback?.Invoke(Context.Activity);
        }

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }
    }
}
