namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Util;


    public class RetryConsumeContext :
        ConsumeContextScope,
        ConsumeRetryContext
    {
        readonly IRetryPolicy _retryPolicy;
        readonly ConsumeContext _context;
        readonly IList<PendingFault> _pendingFaults;

        public RetryConsumeContext(ConsumeContext context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context)
        {
            _retryPolicy = retryPolicy;
            _context = context;

            if (retryContext != null)
            {
                RetryAttempt = retryContext.RetryAttempt;
                RetryCount = retryContext.RetryCount;
            }

            _pendingFaults = new List<PendingFault>();
        }

        public int RetryAttempt { get; }

        public int RetryCount { get; }

        public override Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            if (_retryPolicy.IsHandled(exception))
            {
                _pendingFaults.Add(new PendingFault<T>(context, duration, consumerType, exception));
                return TaskUtil.Completed;
            }

            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public RetryConsumeContext CreateNext(RetryContext retryContext)
        {
            return new RetryConsumeContext(_context, _retryPolicy, retryContext);
        }

        protected IRetryPolicy RetryPolicy => _retryPolicy;

        public virtual TContext CreateNext<TContext>(RetryContext retryContext)
            where TContext : class, ConsumeRetryContext
        {
            throw new InvalidOperationException("This is only supported by a derived type");
        }

        public Task NotifyPendingFaults()
        {
            return Task.WhenAll(_pendingFaults.Select(x => x.Notify(_context)));
        }


        interface PendingFault
        {
            Task Notify(ConsumeContext context);
        }


        class PendingFault<T> :
            PendingFault
            where T : class
        {
            readonly string _consumerType;
            readonly ConsumeContext<T> _context;
            readonly TimeSpan _elapsed;
            readonly Exception _exception;

            public PendingFault(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            {
                _context = context;
                _elapsed = elapsed;
                _consumerType = consumerType;
                _exception = exception;
            }

            public Task Notify(ConsumeContext context)
            {
                return context.NotifyFaulted(_context, _elapsed, _consumerType, _exception);
            }
        }
    }


    public class RetryConsumeContext<T> :
        RetryConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public RetryConsumeContext(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, retryPolicy, retryContext)
        {
            _context = context;
        }

        T ConsumeContext<T>.Message => _context.Message;

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(_context, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(_context, duration, consumerType, exception);
        }

        public override TContext CreateNext<TContext>(RetryContext retryContext)
        {
            return new RetryConsumeContext<T>(_context, RetryPolicy, retryContext) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeMetadataCache<T>.ShortName}");
        }
    }
}
