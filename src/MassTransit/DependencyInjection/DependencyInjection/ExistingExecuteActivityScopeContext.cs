namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingExecuteActivityScopeContext<TActivity, TArguments> :
        IExecuteActivityScopeContext<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public ExistingExecuteActivityScopeContext(ExecuteActivityContext<TActivity, TArguments> context, IServiceScope scope, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public ExecuteActivityContext<TActivity, TArguments> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
            return default;
        }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }
    }
}
