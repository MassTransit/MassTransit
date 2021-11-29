namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class NoRetryContext<TContext> :
        BaseRetryContext<TContext>,
        RetryContext<TContext>
        where TContext : class, PipeContext
    {
        public NoRetryContext(TContext context, Exception exception, CancellationToken cancellationToken)
            : base(context, exception, 0, cancellationToken)
        {
        }

        bool RetryContext<TContext>.CanRetry(Exception exception, out RetryContext<TContext> retryContext)
        {
            retryContext = this;

            return false;
        }
    }
}
