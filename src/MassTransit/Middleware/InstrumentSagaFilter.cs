namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Monitoring;


    public class InstrumentSagaFilter<TSaga, TMessage> :
        IFilter<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            using var inProgress = Instrumentation.TrackSagaInProgress<TSaga, TMessage>();

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("instrument");
        }
    }
}
