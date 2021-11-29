namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IServiceScope _scope;

        public CreatedCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

        public ValueTask DisposeAsync()
        {
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
