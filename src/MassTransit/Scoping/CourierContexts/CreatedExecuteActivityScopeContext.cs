namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using Courier;


    public class CreatedExecuteActivityScopeContext<TScope, TActivity, TArguments> :
        IExecuteActivityScopeContext<TActivity, TArguments>
        where TScope : IDisposable
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly TScope _scope;

        public CreatedExecuteActivityScopeContext(TScope scope, ExecuteActivityContext<TActivity, TArguments> context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }
    }
}