namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    public class FilterPipe<TContext> :
        IPipe<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilter<TContext> _filter;
        readonly IPipe<TContext> _next;

        public FilterPipe(IFilter<TContext> filter, IPipe<TContext> next)
        {
            _filter = filter;
            _next = next;
        }

        public void Probe(ProbeContext context)
        {
            _filter.Probe(context);
            _next.Probe(context);
        }

        [DebuggerStepThrough]
        public Task Send(TContext context)
        {
            return _filter.Send(context, _next);
        }
    }
}
