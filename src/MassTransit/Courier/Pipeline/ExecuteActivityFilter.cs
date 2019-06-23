namespace MassTransit.Courier.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Executes an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class ExecuteActivityFilter<TActivity, TArguments> :
        IFilter<RequestContext<ExecuteActivityContext<TActivity, TArguments>>>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("execute");
        }

        public async Task Send(RequestContext<ExecuteActivityContext<TActivity, TArguments>> context,
            IPipe<RequestContext<ExecuteActivityContext<TActivity, TArguments>>> next)
        {
            var result = await context.Request.Activity.Execute(context.Request).ConfigureAwait(false);

            context.TrySetResult(result);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
