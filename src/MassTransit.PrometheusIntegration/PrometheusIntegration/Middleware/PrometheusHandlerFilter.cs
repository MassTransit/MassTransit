namespace MassTransit.PrometheusIntegration.Middleware
{
    using System.Threading.Tasks;


    public class PrometheusHandlerFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            using var inProgress = PrometheusMetrics.TrackHandlerInProgress<TMessage>();

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("prometheus");
        }
    }
}
