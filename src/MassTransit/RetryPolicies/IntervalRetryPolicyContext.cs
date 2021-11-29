namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class IntervalRetryPolicyContext<TContext> :
        BaseRetryPolicyContext<TContext>
        where TContext : class, PipeContext
    {
        readonly IntervalRetryPolicy _policy;

        public IntervalRetryPolicyContext(IntervalRetryPolicy policy, TContext context)
            : base(policy, context)
        {
            _policy = policy;
        }

        protected override RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken)
        {
            return new IntervalRetryContext<TContext>(_policy, Context, exception, 0, cancellationToken);
        }
    }
}
