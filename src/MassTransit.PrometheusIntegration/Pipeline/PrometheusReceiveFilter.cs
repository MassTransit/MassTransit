namespace MassTransit.PrometheusIntegration.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class PrometheusReceiveFilter :
        IFilter<ReceiveContext>
    {
        public async Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            using var inProgress = PrometheusMetrics.TrackReceiveInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("prometheus");
        }
    }
}
