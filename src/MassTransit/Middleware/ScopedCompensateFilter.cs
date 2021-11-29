namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Courier;
    using DependencyInjection;


    public class ScopedCompensateFilter<TActivity, TArguments, TFilter> :
        IFilter<CompensateContext<TArguments>>
        where TActivity : class, ICompensateActivity<TArguments>
        where TArguments : class
        where TFilter : class, IFilter<CompensateContext<TArguments>>
    {
        readonly ICompensateActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopedCompensateFilter(ICompensateActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(CompensateContext<TArguments> context, IPipe<CompensateContext<TArguments>> next)
        {
            await using ICompensateScopeContext<TArguments> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

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
