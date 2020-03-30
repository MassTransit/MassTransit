namespace MassTransit.Courier.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using GreenPipes;


    /// <summary>
    /// Executes an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class ExecuteActivityFilter<TActivity, TArguments> :
        IFilter<ExecuteActivityContext<TActivity, TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("execute");
        }

        public async Task Send(ExecuteActivityContext<TActivity, TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            context.Result = await context.Activity.Execute(context).ConfigureAwait(false);

            var result = context.Result ?? context.Faulted(new ActivityExecutionException("The activity execute did not return a result"));
            if (result.IsFaulted(out var exception))
            {
                var ex = exception is ExceptionWithVariables exceptionWithVariables ?
                    new AggregateExceptionWithVariables(exceptionWithVariables, exceptionWithVariables.Variables) :
                    new AggregateException(exception);
                throw ex;
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
