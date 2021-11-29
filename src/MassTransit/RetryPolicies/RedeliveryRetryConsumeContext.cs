namespace MassTransit.RetryPolicies
{
    using System;


    public class RedeliveryRetryConsumeContext<T> :
        RetryConsumeContext<T>
        where T : class
    {
        public RedeliveryRetryConsumeContext(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, retryPolicy, retryContext)
        {
        }

        public override TContext CreateNext<TContext>(RetryContext retryContext)
        {
            return this as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeCache<T>.ShortName}");
        }
    }
}
