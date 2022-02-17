namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;


    public class FactoryMethodCompensateActivityFactory<TActivity, TLog> :
        ICompensateActivityFactory<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly Func<TLog, TActivity> _compensateFactory;

        public FactoryMethodCompensateActivityFactory(Func<TLog, TActivity> compensateFactory)
        {
            _compensateFactory = compensateFactory;
        }

        public async Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            TActivity activity = null;
            try
            {
                activity = _compensateFactory(context.Log);

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                await next.Send(activityContext).ConfigureAwait(false);
            }
            finally
            {
                switch (activity)
                {
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
