namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IServiceScope _scope;

        public ExistingCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, IServiceScope scope)
        {
            _scope = scope;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

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
