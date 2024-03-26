namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;


    public class RetryCompensateContext<TLog> :
        CompensateContextScope<TLog>,
        ConsumeRetryContext
        where TLog : class
    {
        readonly CompensateContext<TLog> _context;
        readonly CompensationResult _existingResult;
        readonly IRetryPolicy _retryPolicy;

        public RetryCompensateContext(CompensateContext<TLog> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context)
        {
            _retryPolicy = retryPolicy;
            _context = context;

            if (retryContext is RetryContext<CompensateContext<TLog>> compensateRetryContext)
                _existingResult = compensateRetryContext.Context.Result;

            Result = new RetryCompensationResult();

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
            return new RetryCompensateContext<TLog>(_context, _retryPolicy, retryContext) as TContext;
        }

        public Task NotifyPendingFaults()
        {
            if (_existingResult != null && Result is RetryCompensationResult)
                Result = _existingResult;

            return Task.CompletedTask;
        }


        class RetryCompensationResult :
            CompensationResult
        {
            readonly Exception _exception;

            public RetryCompensationResult(Exception exception = null)
            {
                _exception = exception;
            }

            public Task Evaluate()
            {
                return Task.CompletedTask;
            }

            public bool IsFailed(out Exception exception)
            {
                exception = _exception;
                return exception != null;
            }
        }
    }
}
