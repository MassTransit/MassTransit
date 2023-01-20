namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public ExistingCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, IServiceScope scope, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

        public async ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
        }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }
    }
}
