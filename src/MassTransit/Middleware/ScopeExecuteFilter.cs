namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;
    using DependencyInjection;


    public class ScopeExecuteFilter<TActivity, TArguments> :
        IFilter<ExecuteContext<TArguments>>
        where TArguments : class
        where TActivity : class, IExecuteActivity<TArguments>
    {
        readonly IExecuteActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopeExecuteFilter(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ExecuteContext<TArguments> context, IPipe<ExecuteContext<TArguments>> next)
        {
            await using IExecuteScopeContext<TArguments> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
