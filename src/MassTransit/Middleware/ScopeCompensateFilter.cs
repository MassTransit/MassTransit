namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;
    using DependencyInjection;


    public class ScopeCompensateFilter<TActivity, TLog> :
        IFilter<CompensateContext<TLog>>
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        readonly ICompensateActivityScopeProvider<TActivity, TLog> _scopeProvider;

        public ScopeCompensateFilter(ICompensateActivityScopeProvider<TActivity, TLog> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(CompensateContext<TLog> context, IPipe<CompensateContext<TLog>> next)
        {
            await using ICompensateScopeContext<TLog> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
