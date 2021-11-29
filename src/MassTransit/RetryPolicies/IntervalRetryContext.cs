namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class IntervalRetryContext<TContext> :
        BaseRetryContext<TContext>,
        RetryContext<TContext>
        where TContext : class, PipeContext
    {
        readonly IntervalRetryPolicy _policy;

        public IntervalRetryContext(IntervalRetryPolicy policy, TContext context, Exception exception, int retryCount, CancellationToken cancellationToken)
            : base(context, exception, retryCount, cancellationToken)
        {
            _policy = policy;
        }

        public override TimeSpan? Delay => _policy.Intervals[RetryCount];

        bool RetryContext<TContext>.CanRetry(Exception exception, out RetryContext<TContext> retryContext)
        {
            retryContext = new IntervalRetryContext<TContext>(_policy, Context, Exception, RetryCount + 1, CancellationToken);

            return RetryAttempt < _policy.Intervals.Length && _policy.IsHandled(exception);
        }
    }
}
