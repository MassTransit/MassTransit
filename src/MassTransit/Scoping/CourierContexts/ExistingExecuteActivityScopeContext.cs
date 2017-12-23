namespace MassTransit.Scoping.CourierContexts
{
    using Courier;


    public class ExistingExecuteActivityScopeContext<TActivity, TArguments> :
        IExecuteActivityScopeContext<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExistingExecuteActivityScopeContext(ExecuteActivityContext<TActivity, TArguments> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }
    }
}