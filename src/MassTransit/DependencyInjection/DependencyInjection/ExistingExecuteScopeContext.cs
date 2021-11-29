namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingExecuteScopeContext<TArguments> :
        IExecuteScopeContext<TArguments>
        where TArguments : class
    {
        readonly IServiceScope _scope;

        public ExistingExecuteScopeContext(ExecuteContext<TArguments> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public ExecuteContext<TArguments> Context { get; }

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
