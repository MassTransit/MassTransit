namespace MassTransit.Scoping.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScopedFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilterContextScopeProvider<TContext> _scopeProvider;

        public ScopedFilter(IFilterContextScopeProvider<TContext> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            await using IFilterContextScope<TContext> scope = _scopeProvider.Create(context);

            await scope.Filter.Send(scope.Context, next).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope =context.CreateFilterScope("scopedFilter");

            _scopeProvider.Probe(scope);
        }
    }
}
