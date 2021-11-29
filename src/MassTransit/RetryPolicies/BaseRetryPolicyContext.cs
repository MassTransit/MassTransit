namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public abstract class BaseRetryPolicyContext<TContext> :
        RetryPolicyContext<TContext>
        where TContext : class, PipeContext
    {
        readonly IRetryPolicy _policy;
        CancellationTokenSource _cancellationTokenSource;
        CancellationTokenRegistration _registration;

        protected BaseRetryPolicyContext(IRetryPolicy policy, TContext context)
        {
            _policy = policy;
            Context = context;
        }

        protected CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CreateCancellationToken();

        public TContext Context { get; }

        public virtual bool CanRetry(Exception exception, out RetryContext<TContext> retryContext)
        {
            retryContext = CreateRetryContext(exception, CancellationToken);

            return _policy.IsHandled(exception) && !_cancellationTokenSource.IsCancellationRequested;
        }

        Task RetryPolicyContext<TContext>.RetryFaulted(Exception exception)
        {
            return Task.CompletedTask;
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        void IDisposable.Dispose()
        {
            _registration.Dispose();
        }

        protected abstract RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken);

        CancellationToken CreateCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (Context.CancellationToken.CanBeCanceled)
                _registration = Context.CancellationToken.Register(_cancellationTokenSource.Cancel);

            return _cancellationTokenSource.Token;
        }
    }
}
