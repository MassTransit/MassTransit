namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;


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

        public async Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            TActivity activity = null;
            try
            {
                activity = _executeFactory(context.Arguments);

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                await next.Send(activityContext).ConfigureAwait(false);
            }
            finally
            {
                switch (activity)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factoryMethod");
        }
    }
}
