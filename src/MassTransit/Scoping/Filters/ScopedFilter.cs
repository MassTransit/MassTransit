namespace MassTransit.Scoping.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScopedFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilterContextScopeProvider<TContext> _filterContextScopeProvider;

        public ScopedFilter(IFilterContextScopeProvider<TContext> filterContextScopeProvider)
        {
            _filterContextScopeProvider = filterContextScopeProvider;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            using IFilterContextScope<TContext> scope = _filterContextScopeProvider.Create(context);
            await scope.Filter.Send(scope.Context, next).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
