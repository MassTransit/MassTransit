namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using Middleware;
    using Observables;
    using RetryPolicies;


    public class CompensateContextRetryPipeSpecification<TLog> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<CompensateContext<TLog>>
        where TLog : class
    {
        readonly CancellationToken _cancellationToken;
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public CompensateContextRetryPipeSpecification(CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<CompensateContext<TLog>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<CompensateContext<TLog>, RetryCompensateContext<TLog>>(retryPolicy, _cancellationToken, Factory);

            builder.AddFilter(new RetryFilter<CompensateContext<TLog>>(policy, _observers));
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

        static RetryCompensateContext<TLog> Factory(CompensateContext<TLog> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryCompensateContext<TLog> ?? new RetryCompensateContext<TLog>(context, retryPolicy, retryContext);
        }
    }
}
