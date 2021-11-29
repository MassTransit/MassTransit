namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;
    using DependencyInjection;


    public class ScopedExecuteFilter<TActivity, TArguments, TFilter> :
        IFilter<ExecuteContext<TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
        where TFilter : class, IFilter<ExecuteContext<TArguments>>
    {
        readonly IExecuteActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopedExecuteFilter(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ExecuteContext<TArguments> context, IPipe<ExecuteContext<TArguments>> next)
        {
            await using IExecuteScopeContext<TArguments> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            var filter = scope.GetService<TFilter>();

            await filter.Send(scope.Context, next).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scopedFilter");

            _scopeProvider.Probe(scope);
        }
    }
}
