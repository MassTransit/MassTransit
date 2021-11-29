namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using DependencyInjection;


    public class ScopedFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilterScopeProvider<TContext> _scopeProvider;

        public ScopedFilter(IFilterScopeProvider<TContext> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            await using IFilterScopeContext<TContext> scope = _scopeProvider.Create(context);

            await scope.Filter.Send(scope.Context, next).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scopedFilter");

            _scopeProvider.Probe(scope);
        }
    }
}
