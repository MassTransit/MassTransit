namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Courier;
    using GreenPipes;
    using Scoping;


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
            await using IExecuteActivityScopeContext<TActivity, TArguments> scope = await _scopeProvider.GetScope(context);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
