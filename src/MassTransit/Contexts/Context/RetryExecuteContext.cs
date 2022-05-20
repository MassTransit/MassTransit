namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;


    public class RetryExecuteContext<TArguments> :
        ExecuteContextScope<TArguments>,
        ConsumeRetryContext
        where TArguments : class
    {
        readonly ExecuteContext<TArguments> _context;
        readonly IRetryPolicy _retryPolicy;

        public RetryExecuteContext(ExecuteContext<TArguments> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context)
        {
            _retryPolicy = retryPolicy;
            _context = context;

            context.Result = new RetryExecutionResult();

            if (retryContext != null)
            {
                RetryAttempt = retryContext.RetryAttempt;
                RetryCount = retryContext.RetryCount;
            }
            else if (context.TryGetPayload<ConsumeRetryContext>(out var existingRetryContext))
            {
                RetryCount = existingRetryContext.RetryCount;
                RetryAttempt = existingRetryContext.RetryAttempt;
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
            return Task.CompletedTask;
        }


        class RetryExecutionResult :
            ExecutionResult
        {
            public Task Evaluate()
            {
                return Task.CompletedTask;
            }

            public bool IsFaulted(out Exception exception)
            {
                exception = null;
                return false;
            }
        }
    }
}
