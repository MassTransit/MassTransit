namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;


    public class ImmediateRetryPolicyContext<TContext> :
        BaseRetryPolicyContext<TContext>
        where TContext : class, PipeContext
    {
        readonly ImmediateRetryPolicy _policy;

        public ImmediateRetryPolicyContext(ImmediateRetryPolicy policy, TContext context)
            : base(policy, context)
        {
            _policy = policy;
        }

        protected override RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken)
        {
            return new ImmediateRetryContext<TContext>(_policy, Context, exception, 0, cancellationToken);
        }
    }
}
