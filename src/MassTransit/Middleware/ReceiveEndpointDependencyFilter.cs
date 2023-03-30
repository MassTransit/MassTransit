namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Internals;
    using Transports;


    public class ReceiveEndpointDependencyFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly ReceiveEndpointContext _context;

        public ReceiveEndpointDependencyFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            await _context.DependenciesReady.OrCanceled(context.CancellationToken).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiveEndpointDependencies");
            scope.Add("contextType", typeof(TContext).Name);
        }
    }
}
