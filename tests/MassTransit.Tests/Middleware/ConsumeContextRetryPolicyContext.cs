namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    public class CommandContextRetryPolicyContext :
        RetryPolicyContext<CommandContext>
    {
        readonly RetryCommandContext _context;
        readonly RetryPolicyContext<CommandContext> _policyContext;

        public CommandContextRetryPolicyContext(RetryPolicyContext<CommandContext> policyContext, RetryCommandContext context)
        {
            _policyContext = policyContext;
            _context = context;
        }

        public CommandContext Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<CommandContext> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<CommandContext> policyRetryContext);

            retryContext = new ConsumeContextRetryContext(policyRetryContext, canRetry ? _context.CreateNext() : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return _policyContext.RetryFaulted(exception);
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public void Dispose()
        {
            _policyContext?.Dispose();
        }
    }


    public class CommandContextRetryPolicyContext<TFilter, TContext> :
        RetryPolicyContext<TFilter>
        where TFilter : class, CommandContext
        where TContext : RetryCommandContext, TFilter
    {
        readonly TContext _context;
        readonly RetryPolicyContext<TFilter> _policyContext;

        public CommandContextRetryPolicyContext(RetryPolicyContext<TFilter> policyContext, TContext context)
        {
            _policyContext = policyContext;
            _context = context;
        }

        public TFilter Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<TFilter> policyRetryContext);

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext, canRetry ? _context.CreateNext<TContext>() : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return _policyContext.RetryFaulted(exception);
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public void Dispose()
        {
            _policyContext?.Dispose();
        }
    }
}
