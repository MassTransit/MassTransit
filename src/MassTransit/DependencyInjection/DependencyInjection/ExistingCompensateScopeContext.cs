namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingCompensateScopeContext<TLog> :
        ICompensateScopeContext<TLog>
        where TLog : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public ExistingCompensateScopeContext(CompensateContext<TLog> context, IServiceScope scope, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public CompensateContext<TLog> Context { get; }

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
