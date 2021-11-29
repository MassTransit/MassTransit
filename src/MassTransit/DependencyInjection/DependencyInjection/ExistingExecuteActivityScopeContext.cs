namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingExecuteActivityScopeContext<TActivity, TArguments> :
        IExecuteActivityScopeContext<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IServiceScope _scope;

        public ExistingExecuteActivityScopeContext(ExecuteActivityContext<TActivity, TArguments> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }
    }
}
