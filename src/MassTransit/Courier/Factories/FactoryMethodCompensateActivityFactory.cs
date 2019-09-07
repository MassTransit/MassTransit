namespace MassTransit.Courier.Factories
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosts;


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

        public async Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            TActivity activity = null;
            try
            {
                activity = _compensateFactory(context.Log);

                var activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

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
