namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedExecuteScopeContext<TArguments> :
        IExecuteScopeContext<TArguments>
        where TArguments : class
    {
        readonly IServiceScope _scope;

        public CreatedExecuteScopeContext(ExecuteContext<TArguments> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public ExecuteContext<TArguments> Context { get; }

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
