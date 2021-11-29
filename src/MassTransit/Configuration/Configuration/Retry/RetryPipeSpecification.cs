namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;
    using Observables;


    public class RetryPipeSpecification<TContext> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public RetryPipeSpecification()
        {
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<TContext> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            builder.AddFilter(new RetryFilter<TContext>(retryPolicy, _observers));
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

        public ConnectHandle ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
