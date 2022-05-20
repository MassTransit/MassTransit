namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Monitoring;


    public class InstrumentConsumerFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            using var inProgress = Instrumentation.TrackConsumerInProgress<TConsumer, TMessage>();

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("instrument");
        }
    }
}
