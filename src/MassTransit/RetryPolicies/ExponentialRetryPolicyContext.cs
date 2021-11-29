namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class ExponentialRetryPolicyContext<TContext> :
        BaseRetryPolicyContext<TContext>
        where TContext : class, PipeContext
    {
        readonly ExponentialRetryPolicy _policy;

        public ExponentialRetryPolicyContext(ExponentialRetryPolicy policy, TContext context)
            : base(policy, context)
        {
            _policy = policy;
        }

        protected override RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken)
        {
            return new ExponentialRetryContext<TContext>(_policy, Context, exception, 0, cancellationToken);
        }
    }
}
