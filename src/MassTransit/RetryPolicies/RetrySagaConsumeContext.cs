namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading.Tasks;


    public class RetrySagaConsumeContext<TSaga> :
        RetryConsumeContext,
        SagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;

        public RetrySagaConsumeContext(SagaConsumeContext<TSaga> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, retryPolicy, retryContext)
        {
            _context = context;
        }

        public TSaga Saga => _context.Saga;

        Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        public override TContext CreateNext<TContext>(RetryContext retryContext)
        {
            return new RetrySagaConsumeContext<TSaga>(_context, RetryPolicy, retryContext) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeCache<TContext>.ShortName}");
        }
    }
}
