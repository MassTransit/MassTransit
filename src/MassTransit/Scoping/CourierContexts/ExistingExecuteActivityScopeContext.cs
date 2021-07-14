namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using System.Threading.Tasks;
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

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposeCallback?.Invoke(Context.Activity);

            return default;
        }
    }
}
