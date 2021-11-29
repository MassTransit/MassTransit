namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using Middleware;
    using Observables;
    using RetryPolicies;


    public class ExecuteContextRetryPipeSpecification<TArguments> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<ExecuteContext<TArguments>>
        where TArguments : class
    {
        readonly CancellationToken _cancellationToken;
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public ExecuteContextRetryPipeSpecification(CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<ExecuteContext<TArguments>, RetryExecuteContext<TArguments>>(retryPolicy, _cancellationToken, Factory);

            builder.AddFilter(new RetryFilter<ExecuteContext<TArguments>>(policy, _observers));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_policyFactory == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }

        public void SetRetryPolicy(RetryPolicyFactory factory)
        {
            _policyFactory = factory;
        }

        ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }

        static RetryExecuteContext<TArguments> Factory(ExecuteContext<TArguments> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryExecuteContext<TArguments> ?? new RetryExecuteContext<TArguments>(context, retryPolicy, retryContext);
        }
    }
}
