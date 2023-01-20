namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingExecuteScopeContext<TArguments> :
        IExecuteScopeContext<TArguments>
        where TArguments : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public ExistingExecuteScopeContext(ExecuteContext<TArguments> context, IServiceScope scope, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public ExecuteContext<TArguments> Context { get; }

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
