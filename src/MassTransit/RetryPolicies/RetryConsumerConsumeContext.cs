namespace MassTransit.RetryPolicies
{
    using System;


    public class RetryConsumerConsumeContext<TConsumer> :
        RetryConsumeContext,
        ConsumerConsumeContext<TConsumer>
        where TConsumer : class
    {
        readonly ConsumerConsumeContext<TConsumer> _context;

        public RetryConsumerConsumeContext(ConsumerConsumeContext<TConsumer> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, retryPolicy, retryContext)
        {
            _context = context;
        }

        public TConsumer Consumer => _context.Consumer;

        public override TContext CreateNext<TContext>(RetryContext retryContext)
        {
            return new RetryConsumerConsumeContext<TConsumer>(_context, RetryPolicy, retryContext) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeCache<TContext>.ShortName}");
        }
    }
}
