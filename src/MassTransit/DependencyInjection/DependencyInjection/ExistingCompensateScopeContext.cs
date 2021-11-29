namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingCompensateScopeContext<TLog> :
        ICompensateScopeContext<TLog>
        where TLog : class
    {
        readonly IServiceScope _scope;

        public ExistingCompensateScopeContext(CompensateContext<TLog> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public CompensateContext<TLog> Context { get; }

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
