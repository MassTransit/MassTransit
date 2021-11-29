namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public class ConsumeContextRetryContext :
        RetryContext<ConsumeContext>
    {
        readonly RetryConsumeContext _context;
        readonly RetryContext<ConsumeContext> _retryContext;

        public ConsumeContextRetryContext(RetryContext<ConsumeContext> retryContext, RetryConsumeContext context)
        {
            _retryContext = retryContext;
            _context = context;
        }

        public CancellationToken CancellationToken => _retryContext.CancellationToken;

        public ConsumeContext Context => _context;

        public Exception Exception => _retryContext.Exception;

        public int RetryCount => _retryContext.RetryCount;

        public int RetryAttempt => _retryContext.RetryAttempt;

        public Type ContextType => _retryContext.ContextType;

        public TimeSpan? Delay => _retryContext.Delay;

        public async Task PreRetry()
        {
            await _retryContext.PreRetry().ConfigureAwait(false);
        }

        public async Task RetryFaulted(Exception exception)
        {
            await _retryContext.RetryFaulted(exception).ConfigureAwait(false);

            await _context.NotifyPendingFaults().ConfigureAwait(false);
        }

        public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
        {
            var canRetry = _retryContext.CanRetry(exception, out RetryContext<ConsumeContext> policyRetryContext);

            retryContext = new ConsumeContextRetryContext(policyRetryContext, canRetry ? _context.CreateNext(policyRetryContext) : _context);

            return canRetry;
        }
    }


    public class ConsumeContextRetryContext<TFilter, TContext> :
        RetryContext<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : class, TFilter, ConsumeRetryContext
    {
        readonly TContext _context;
        readonly RetryContext<TFilter> _retryContext;

        public ConsumeContextRetryContext(RetryContext<TFilter> retryContext, TContext context)
        {
            _retryContext = retryContext;
            _context = context;
        }

        public CancellationToken CancellationToken => _retryContext.CancellationToken;

        public TFilter Context => _context;

        public Exception Exception => _retryContext.Exception;

        public int RetryCount => _retryContext.RetryCount;

        public int RetryAttempt => _retryContext.RetryAttempt;

        public Type ContextType => _retryContext.ContextType;

        public TimeSpan? Delay => _retryContext.Delay;

        public async Task PreRetry()
        {
            await _retryContext.PreRetry().ConfigureAwait(false);
        }

        public async Task RetryFaulted(Exception exception)
        {
            await _retryContext.RetryFaulted(exception).ConfigureAwait(false);

            await _context.NotifyPendingFaults().ConfigureAwait(false);
        }

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            var canRetry = _retryContext.CanRetry(exception, out RetryContext<TFilter> policyRetryContext);

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext,
                canRetry ? _context.CreateNext<TContext>(policyRetryContext) : _context);

            return canRetry;
        }
    }
}
