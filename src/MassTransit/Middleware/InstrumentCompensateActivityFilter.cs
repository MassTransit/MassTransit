namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Monitoring;


    public class InstrumentCompensateActivityFilter<TActivity, TLog> :
        IFilter<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            using var inProgress = Instrumentation.TrackCompensateActivityInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("instrument");
        }
    }
}
