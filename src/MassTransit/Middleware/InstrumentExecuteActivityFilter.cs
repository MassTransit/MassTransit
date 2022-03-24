namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Monitoring;


    public class InstrumentExecuteActivityFilter<TActivity, TArguments> :
        IFilter<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public async Task Send(ExecuteActivityContext<TActivity, TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            using var inProgress = Instrumentation.TrackExecuteActivityInProgress(context);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("instrument");
        }
    }
}
