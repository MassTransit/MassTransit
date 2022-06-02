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
        readonly ExecutionResult _existingResult;
        readonly IRetryPolicy _retryPolicy;

        public RetryExecuteContext(ExecuteContext<TArguments> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context)
        {
            _retryPolicy = retryPolicy;
            _context = context;

            if (retryContext is RetryContext<ExecuteContext<TArguments>> executeRetryContext)
                _existingResult = executeRetryContext.Context.Result;

            Result = new RetryExecutionResult();

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
            if (_existingResult != null && Result is RetryExecutionResult)
                Result = _existingResult;

            return Task.CompletedTask;
        }


        class RetryExecutionResult :
            ExecutionResult
        {
            readonly Exception _exception;

            public RetryExecutionResult(Exception exception = null)
            {
                _exception = exception;
            }

            public Task Evaluate()
            {
                return Task.CompletedTask;
            }

            public bool IsFaulted(out Exception exception)
            {
                exception = _exception;
                return exception != null;
            }
        }
    }
}
