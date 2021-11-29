namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedCompensateScopeContext<TLog> :
        ICompensateScopeContext<TLog>
        where TLog : class
    {
        readonly IServiceScope _scope;

        public CreatedCompensateScopeContext(IServiceScope scope, CompensateContext<TLog> context)
        {
            _scope = scope;
            Context = context;
        }

        public CompensateContext<TLog> Context { get; }

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
