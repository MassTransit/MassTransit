namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;


    public class TransportReadyFilter<T> :
        Agent,
        IFilter<T>
        where T : class, PipeContext
    {
        readonly ReceiveEndpointContext _context;

        public TransportReadyFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(T context, IPipe<T> next)
        {
            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            SetReady();

            await next.Send(context).ConfigureAwait(false);

            await Completed.ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("transportReady");
        }
    }
}
