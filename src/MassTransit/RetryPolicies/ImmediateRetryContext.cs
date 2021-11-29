namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class ImmediateRetryContext<TContext> :
        BaseRetryContext<TContext>,
        RetryContext<TContext>
        where TContext : class, PipeContext
    {
        readonly ImmediateRetryPolicy _policy;

        public ImmediateRetryContext(ImmediateRetryPolicy policy, TContext context, Exception exception, int retryCount, CancellationToken cancellationToken)
            : base(context, exception, retryCount, cancellationToken)
        {
            _policy = policy;
        }

        bool RetryContext<TContext>.CanRetry(Exception exception, out RetryContext<TContext> retryContext)
        {
            retryContext = new ImmediateRetryContext<TContext>(_policy, Context, Exception, RetryCount + 1, CancellationToken);

            return RetryAttempt < _policy.RetryLimit && _policy.IsHandled(exception);
        }
    }
}
