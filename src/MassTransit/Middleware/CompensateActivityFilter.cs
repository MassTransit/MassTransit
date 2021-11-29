namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Internals;
    using Observables;


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
        readonly ActivityObservable _observers;

        public CompensateActivityFilter(ActivityObservable observers)
        {
            _observers = observers;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("compensate");
        }

        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            try
            {
                if (_observers.Count > 0)
                    await _observers.PreCompensate(context).ConfigureAwait(false);

                var result = context.Result = await context.Activity.Compensate(context).ConfigureAwait(false)
                    ?? context.Failed(new ActivityCompensationException("The activity compensation did not return a result"));

                if (result.IsFailed(out var exception))
                    exception.Rethrow();

                await next.Send(context).ConfigureAwait(false);

                if (_observers.Count > 0)
                    await _observers.PostCompensate(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (context.Result == null || !context.Result.IsFailed(out var faultException) || faultException != exception)
                    context.Result = context.Failed(exception);

                if (_observers.Count > 0)
                    await _observers.CompensateFail(context, exception).ConfigureAwait(false);

                throw;
            }
        }
    }
}
