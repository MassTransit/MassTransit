namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Monitoring;


    public class InstrumentReceiveFilter :
        IFilter<ReceiveContext>
    {
        public async Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            using var inProgress = Instrumentation.TrackReceiveInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("instrument");
        }
    }
}
