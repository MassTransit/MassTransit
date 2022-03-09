namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class BaseRetryContext<TContext> :
        RetryContext
        where TContext : class, PipeContext
    {
        protected BaseRetryContext(TContext context, Exception exception, int retryCount, CancellationToken cancellationToken)
        {
            Context = context;
            Exception = exception;
            CancellationToken = cancellationToken;

            RetryCount = retryCount;
            RetryAttempt = retryCount + 1;
        }

        public TContext Context { get; }

        public CancellationToken CancellationToken { get; }

        public Exception Exception { get; }

        public int RetryAttempt { get; }

        public int RetryCount { get; }

        public virtual TimeSpan? Delay => default;

        Type RetryContext.ContextType => typeof(TContext);

        public virtual Task PreRetry()
        {
            return Task.CompletedTask;
        }

        public virtual Task RetryFaulted(Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
