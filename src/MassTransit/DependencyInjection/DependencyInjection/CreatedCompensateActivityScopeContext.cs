namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public CreatedCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, IServiceScope scope, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();

            if (_scope is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            _scope?.Dispose();
            return default;
        }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }
    }
}
