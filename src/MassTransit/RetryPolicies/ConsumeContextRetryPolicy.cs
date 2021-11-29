namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using Context;


    public class ConsumeContextRetryPolicy :
        IRetryPolicy
    {
        readonly CancellationToken _cancellationToken;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            _retryPolicy = retryPolicy;
            _cancellationToken = cancellationToken;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            if (context is ConsumeContext consumeContext)
            {
                RetryPolicyContext<ConsumeContext> retryPolicyContext = _retryPolicy.CreatePolicyContext(consumeContext);

                var retryConsumeContext = new RetryConsumeContext(consumeContext, _retryPolicy, null);

                return new ConsumeContextRetryPolicyContext(retryPolicyContext, retryConsumeContext, _cancellationToken) as RetryPolicyContext<T>;
            }

            throw new ArgumentException("The argument must be a ConsumeContext", nameof(context));
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }


    public class ConsumeContextRetryPolicy<TFilter, TContext> :
        IRetryPolicy
        where TFilter : class, ConsumeContext
        where TContext : class, TFilter, ConsumeRetryContext
    {
        readonly CancellationToken _cancellationToken;
        readonly Func<TFilter, IRetryPolicy, RetryContext, TContext> _contextFactory;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken,
            Func<TFilter, IRetryPolicy, RetryContext, TContext> contextFactory)
        {
            _retryPolicy = retryPolicy;
            _cancellationToken = cancellationToken;
            _contextFactory = contextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            var filterContext = context as TFilter;
            if (filterContext == null)
                throw new ArgumentException($"The argument must be a {typeof(TFilter).Name}", nameof(context));

            RetryPolicyContext<TFilter> retryPolicyContext = _retryPolicy.CreatePolicyContext(filterContext);

            var retryConsumeContext = _contextFactory(filterContext, _retryPolicy, null);

            return new ConsumeContextRetryPolicyContext<TFilter, TContext>(retryPolicyContext, retryConsumeContext,
                _cancellationToken) as RetryPolicyContext<T>;
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }
}
