namespace MassTransit.Courier.Contexts
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Util;


    public class RetryExecuteContext<TArguments> :
        ExecuteContextProxy<TArguments>,
        ConsumeRetryContext
        where TArguments : class
    {
        readonly IRetryPolicy _retryPolicy;
        readonly ExecuteContext<TArguments> _context;

        public RetryExecuteContext(ExecuteContext<TArguments> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, new PayloadCacheScope(context))
        {
            _retryPolicy = retryPolicy;
            _context = context;

            if (retryContext != null)
            {
                RetryAttempt = retryContext.RetryAttempt;
                RetryCount = retryContext.RetryCount;
            }
        }

        public int RetryAttempt { get; }

        public int RetryCount { get; }

        public TContext CreateNext<TContext>(RetryContext retryContext)
            where TContext : class, ConsumeRetryContext
        {
            return new RetryExecuteContext<TArguments>(_context, _retryPolicy, retryContext) as TContext;
        }

        public Task NotifyPendingFaults()
        {
            return TaskUtil.Completed;
        }
    }
}
