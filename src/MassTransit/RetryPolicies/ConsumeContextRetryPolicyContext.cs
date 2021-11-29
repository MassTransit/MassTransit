namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Transports;


    public class ConsumeContextRetryPolicyContext :
        RetryPolicyContext<ConsumeContext>
    {
        readonly RetryConsumeContext _context;
        readonly RetryPolicyContext<ConsumeContext> _policyContext;
        CancellationToken _cancellationToken;
        CancellationTokenRegistration _registration;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<ConsumeContext> policyContext, RetryConsumeContext context,
            CancellationToken cancellationToken)
        {
            _policyContext = policyContext;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public ConsumeContext Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<ConsumeContext> policyRetryContext);
            if (canRetry)
            {
                _context.LogRetry(exception);
                _registration = _cancellationToken.Register(Cancel);
            }

            retryContext = new ConsumeContextRetryContext(policyRetryContext, canRetry ? _context.CreateNext(policyRetryContext) : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }

        public void Dispose()
        {
            _registration.Dispose();
            _policyContext.Dispose();
        }
    }


    public class ConsumeContextRetryPolicyContext<TFilter, TContext> :
        RetryPolicyContext<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : class, TFilter, ConsumeRetryContext
    {
        readonly TContext _context;
        readonly RetryPolicyContext<TFilter> _policyContext;
        CancellationToken _cancellationToken;
        CancellationTokenRegistration _registration;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<TFilter> policyContext, TContext context, CancellationToken cancellationToken)
        {
            _policyContext = policyContext;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public TFilter Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<TFilter> policyRetryContext);
            if (canRetry)
            {
                _context.LogRetry(exception);
                _registration = _cancellationToken.Register(Cancel);
            }

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext,
                canRetry ? _context.CreateNext<TContext>(policyRetryContext) : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }

        public void Dispose()
        {
            _registration.Dispose();
            _policyContext.Dispose();
        }
    }
}
