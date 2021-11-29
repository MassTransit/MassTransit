namespace MassTransit.PrometheusIntegration.Middleware
{
    using System.Threading.Tasks;
    using Courier;


    public class PrometheusCompensateActivityFilter<TActivity, TLog> :
        IFilter<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            using var inProgress = PrometheusMetrics.TrackCompensateActivityInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("prometheus");
        }
    }
}
