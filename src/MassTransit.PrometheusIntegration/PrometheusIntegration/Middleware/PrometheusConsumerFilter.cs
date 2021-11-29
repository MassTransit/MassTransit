namespace MassTransit.PrometheusIntegration.Middleware
{
    using System.Threading.Tasks;


    public class PrometheusConsumerFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            using var inProgress = PrometheusMetrics.TrackConsumerInProgress<TConsumer, TMessage>();

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("prometheus");
        }
    }
}
