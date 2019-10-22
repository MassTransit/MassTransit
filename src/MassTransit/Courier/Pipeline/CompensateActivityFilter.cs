namespace MassTransit.Courier.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using GreenPipes;


    /// <summary>
    /// Compensates an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class CompensateActivityFilter<TActivity, TLog> :
        IFilter<CompensateActivityContext<TActivity, TLog>>
        where TLog : class
        where TActivity : class, ICompensateActivity<TLog>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("compensate");
        }

        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            context.Result = await context.Activity.Compensate(context).ConfigureAwait(false);

            var result = context.Result ?? context.Failed(new ActivityCompensationException("The activity compensation did not return a result"));
            if (result.IsFailed(out var exception))
                throw new AggregateException(exception);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
