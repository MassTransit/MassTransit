namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class IncrementalRetryContext<TContext> :
        BaseRetryContext<TContext>,
        RetryContext<TContext>
        where TContext : class, PipeContext
    {
        readonly TimeSpan _delay;
        readonly TimeSpan _delayIncrement;
        readonly IncrementalRetryPolicy _policy;

        public IncrementalRetryContext(IncrementalRetryPolicy policy, TContext context, Exception exception, int retryCount, TimeSpan delay,
            TimeSpan delayIncrement, CancellationToken cancellationToken)
            : base(context, exception, retryCount, cancellationToken)
        {
            _policy = policy;
            _delay = delay;
            _delayIncrement = delayIncrement;
        }

        public override TimeSpan? Delay => _delay;

        bool RetryContext<TContext>.CanRetry(Exception exception, out RetryContext<TContext> retryContext)
        {
            retryContext = new IncrementalRetryContext<TContext>(_policy, Context, Exception, RetryCount + 1, _delay + _delayIncrement, _delayIncrement,
                CancellationToken);

            return RetryAttempt < _policy.RetryLimit && _policy.IsHandled(exception);
        }
    }
}
