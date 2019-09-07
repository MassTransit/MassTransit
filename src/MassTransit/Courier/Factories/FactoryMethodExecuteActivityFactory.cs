namespace MassTransit.Courier.Factories
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosts;


    public class FactoryMethodExecuteActivityFactory<TActivity, TArguments> :
        IExecuteActivityFactory<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Func<TArguments, TActivity> _executeFactory;

        public FactoryMethodExecuteActivityFactory(Func<TArguments, TActivity> executeFactory)
        {
            _executeFactory = executeFactory;
        }

        public async Task<ResultContext<ExecutionResult>> Execute(ExecuteContext<TArguments> context,
            IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next)
        {
            TActivity activity = null;
            try
            {
                activity = _executeFactory(context.Arguments);

                var activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
            finally
            {
                var disposable = activity as IDisposable;
                disposable?.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factoryMethod");
        }
    }
}
