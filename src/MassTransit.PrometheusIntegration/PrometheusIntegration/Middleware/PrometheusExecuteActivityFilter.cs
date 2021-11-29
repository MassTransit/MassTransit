namespace MassTransit.PrometheusIntegration.Middleware
{
    using System.Threading.Tasks;
    using Courier;


    public class PrometheusExecuteActivityFilter<TActivity, TArguments> :
        IFilter<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public async Task Send(ExecuteActivityContext<TActivity, TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            using var inProgress = PrometheusMetrics.TrackExecuteActivityInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("prometheus");
        }
    }
}
