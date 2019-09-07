namespace MassTransit.Courier.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Compensates an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class CompensateActivityFilter<TActivity, TLog> :
        IFilter<RequestContext<CompensateActivityContext<TActivity, TLog>>>
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("compensate");
        }

        public async Task Send(RequestContext<CompensateActivityContext<TActivity, TLog>> context,
            IPipe<RequestContext<CompensateActivityContext<TActivity, TLog>>> next)
        {
            var result = await context.Request.Activity.Compensate(context.Request).ConfigureAwait(false);

            context.TrySetResult(result);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
